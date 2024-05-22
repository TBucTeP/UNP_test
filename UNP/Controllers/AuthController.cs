using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using UNP.Models;
using System.Threading.Tasks;

namespace UNP.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUserModel> _signInManager;

        public AuthController(SignInManager<ApplicationUserModel> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Учетная запись заблокирована.");
                    return View("Index", model);
                }
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Вход в учетную запись не разрешен.");
                    return View("Index", model);
                }
            }
            ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
