using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StateManagement.Models;

namespace StateManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        private int a = 0;
        [HttpPost]
        public IActionResult SetA()
        {
            a = 10;
            ViewBag.AValue = "A has been set to 10";
            return View("Index");
        }
        [HttpPost]
        public IActionResult GetA()
        {
            ViewBag.AValue = $"A is currently: {a}";
            return View("Index");
        }
        public IActionResult Index()
        {
            TempData["myKey"] = "Data from Index method";
            return View();
        }
        public IActionResult Index2()
        {
            //HttpContext.Session.Remove("myKey");
            //TempData.Peek("myKey");
            TempData.Keep("myKey");
            ViewBag.MyKey = TempData["myKey"];
            return View();
        }
        public IActionResult Index3()
        {

            ViewBag.Mykey = TempData["myKey"];

            return View();
        }

        public IActionResult Index4()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
