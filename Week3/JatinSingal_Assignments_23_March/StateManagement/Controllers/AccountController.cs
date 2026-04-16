using Microsoft.AspNetCore.Mvc;
using StateManagement.Models;

namespace StateManagement.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Store username in session
                HttpContext.Session.SetString("UserName", model.Username ?? "");

                return RedirectToAction("Welcome");
            }

            return View(model);
        }

        public IActionResult Welcome()
        {
            var username = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(username))
            {
                ViewBag.UserName = username;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login");
        }
    }
}