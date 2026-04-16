using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
namespace ProductAPIClient.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            return View();
        }
    }
}
