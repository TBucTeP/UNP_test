using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UNP.Models;
using UNP.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Net;

namespace UNP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var userHistories = _context.UnpHistories
                    .Where(h => h.Email == email)
                    .OrderByDescending(h => h.LastChecked)
                    .ToList();
                ViewData["UserHistories"] = userHistories;
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnpEntry(UnpRequestModel model)
        {
            var results = new List<object>();

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            string[] unps = model.Unps.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var httpClient = _httpClientFactory.CreateClient();

            foreach (var unp in unps)
            {
                if (!Regex.IsMatch(unp, @"^\d+$"))
                {
                    return BadRequest("Invalid UNP format.");
                }

                var storageRecord = await _context.UnpDatas.FirstOrDefaultAsync(u => u.Vunp == unp);
                bool isInLocalDb = storageRecord != null;
                bool isInExternalDb = false;

                var response = await httpClient.GetAsync($"http://grp.nalog.gov.by/api/grp-public/data?unp={unp}&charset=UTF-8&type=json");
                isInExternalDb = response.IsSuccessStatusCode;

                var email = User.FindFirstValue(ClaimTypes.Email);
                var historyRecord = new UnpHistoryModel
                {
                    Unp = unp,
                    Email = email,
                    IsInLocalDb = isInLocalDb,
                    IsInExternalDb = isInExternalDb,
                    LastChecked = DateTime.Now
                };
                _context.UnpHistories.Add(historyRecord);
                await _context.SaveChangesAsync();

                results.Add(new
                {
                    Unp = unp,
                    LastChecked = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsInLocalDb = isInLocalDb ? "✔️" : "❌",
                    IsInExternalDb = isInExternalDb ? "✔️" : "❌"
                });
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetUnpDetails(string unp)
        {
            var unpDetails = await _context.UnpDatas.FirstOrDefaultAsync(u => u.Vunp == unp);

            if (unpDetails == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Id = unpDetails.Id,
                Vunp = unpDetails.Vunp,
                Vnaimp = unpDetails.Vnaimp,
                Vnaimk = unpDetails.Vnaimk,
                Dreg = unpDetails.Dreg,
                Nmns = unpDetails.Nmns,
                Vmns = unpDetails.Vmns,
                Ckodsost = unpDetails.Ckodsost,
                Dlikv = unpDetails.Dlikv,
                Vlikv = unpDetails.Vlikv,
                LastChecked = unpDetails.LastChecked
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnpDetailsFromExternalApi(string unp)
        {
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.GetAsync($"http://grp.nalog.gov.by/api/grp-public/data?unp={unp}&charset=UTF-8&type=json");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
