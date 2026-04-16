using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PartialViewCGDemo.Models;

namespace PartialViewCGDemo.Controllers
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
        public IActionResult Index2()
        {
            return View();
        }
        Employee emp = new Employee()
        {
            EmpId = 101,
            EmpName = "ravi",
            Email = "ravi@gmail.com",
            Description = "ravi in good working boy"
        };
        public IActionResult displayemp()
        {
            return View(emp);
        }
        public ActionResult DisplayAllEmp()
        {
            List<Employee> emplist = new List<Employee>()
           {
               new Employee{EmpId=101,EmpName="raghavendra ponde",Email="raghuponde@yahoo.com"
               ,Description="I am freelance trainer etc an etcI am freelance trainer etc an etcI am" +
   " freelance trainer etc an etcI am freelance trainer etc an etcI am" +
" freelance trainer etc an etcI am freelance trainer etc an etc" },

               new Employee{EmpId=102,EmpName="suresh",Email="suresh@yahoo.com"
               ,Description="I am freelance trainer etc an etcI am freelance trainer etc an etcI am" +
   " freelance trainer etc an etcI am freelance trainer etc an etcI am" +
" freelance trainer etc an etcI am freelance trainer etc an etc" },

               new Employee{EmpId=103,EmpName="kiran",Email="kiran@yahoo.com"
               ,Description="I am freelance trainer etc an etcI am freelance trainer etc an etcI am" +
   " freelance trainer etc an etcI am freelance trainer etc an etcI am" +
" freelance trainer etc an etcI am freelance trainer etc an etc" },

               new Employee{EmpId=104,EmpName="mahesh",Email="mahesh@yahoo.com"
               ,Description="I am freelance trainer etc an etcI am freelance trainer etc an etcI am" +
   " freelance trainer etc an etcI am freelance trainer etc an etcI am" +
" freelance trainer etc an etcI am freelance trainer etc an etc" }

           };


            return View(emplist);
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
