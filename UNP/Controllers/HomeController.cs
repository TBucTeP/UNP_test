using Microsoft.AspNetCore.Mvc;
using UNP.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net;
using UNP.Data.Repo.Interfaces;

namespace UNP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnpHistoryRepository _unpHistoryRepository;
        private readonly IUnpRepository _unpRepository;


        public HomeController(IUnpRepository unpRepository, IHttpClientFactory httpClientFactory, IUnpHistoryRepository unpHistoryRepository)
        {
            _httpClientFactory = httpClientFactory;
            _unpHistoryRepository = unpHistoryRepository;
            _unpRepository = unpRepository;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var userHistories = await _unpHistoryRepository.GetUserHistoriesByEmailAsync(email);
                return View(userHistories);
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnpEntry(UnpRequestModel model)
        {
            var results = new List<UnpEntryResponseModel>();

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            string[] unps = model.Unps.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var httpClient = _httpClientFactory.CreateClient();

            foreach (var unp in unps)
            {
                var storageRecord = await _unpRepository.GetUnpDataAsync(unp);
                bool isInLocalDb = storageRecord != null;
                bool isInExternalDb = false;

                try
                {
                    var response = await httpClient.GetAsync($"http://grp.nalog.gov.by/api/grp-public/data?unp={unp}&charset=UTF-8&type=json");
                    isInExternalDb = response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, $"Error accessing external API: {ex.Message}");
                }

                var email = User.FindFirstValue(ClaimTypes.Email);
                var historyRecord = new UnpHistoryModel
                {
                    Unp = unp,
                    Email = email,
                    IsInLocalDb = isInLocalDb,
                    IsInExternalDb = isInExternalDb,
                    LastChecked = DateTime.Now
                };

                await _unpRepository.AddUnpHistoryAsync(historyRecord);

                results.Add(new UnpEntryResponseModel
                {
                    Unp = unp,
                    LastChecked = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsInLocalDb = isInLocalDb ? "✔️" : "❌",
                    IsInExternalDb = isInExternalDb ? "✔️" : "❌"
                });
            }
            return Ok(results);
        }

        [HttpGet]
        public async Task<ActionResult<UnpStorageModel>> GetUnpDetails(string unp)
        {
            var unpDetails = await _unpRepository.GetUnpDataAsync(unp);

            if (unpDetails == null)
            {
                return NotFound();
            }

            var result = new UnpStorageModel
            {
                Id = unpDetails.Id,
                Vunp = unpDetails.Vunp,
                Vnaimp = unpDetails.Vnaimp,
                Vnaimk = unpDetails.Vnaimk,
                Vpadres = unpDetails.Vpadres,
                Dreg = unpDetails.Dreg,
                Nmns = unpDetails.Nmns,
                Vmns = unpDetails.Vmns,
                Ckodsost = unpDetails.Ckodsost,
                Dlikv = unpDetails.Dlikv,
                Vlikv = unpDetails.Vlikv,
                LastChecked = unpDetails.LastChecked
            };
            return Ok(result);
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
