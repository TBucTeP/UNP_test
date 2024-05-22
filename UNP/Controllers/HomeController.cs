using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UNP.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace UNP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult UnpEntry(UnpRequestModel model)
        {
            if (ModelState.IsValid)
            {
                // Обработка введенных УНП и отправка уведомлений
                // (реализация зависит от ваших требований)
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
