using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCExampleDemo.Models;

namespace MVCExampleDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public string sampledemo2(int age, string name)
        {
            return "The name " + name + "having age" + age;
        }
        public string sampledemo1()
        {
            return "Jatin";
        }
        Employee obj = new Employee()
        {
            EmployeeID = 101,
            EmpName = "ravi",
            Salary = 34000
        };
        List<Employee> emplist = new List<Employee>()
        {
            new Employee
            {
                EmployeeID = 101,
                EmpName = "GT",
                Salary = 34000,
                ImageUrl = "/images/download.jpg"
            },
            new Employee
            {
                EmployeeID = 102,
                EmpName = "Meteor",
                Salary = 34000,
                ImageUrl = "/images/image1.jpg"
            },
            new Employee
            {
                EmployeeID = 103,
                EmpName = "Bullet",
                Salary = 34000,
                ImageUrl = "/images/images.jpg"
            },
            new Employee
            {
                EmployeeID = 104,
                EmpName = "Hunter",
                Salary = 34000,
                ImageUrl = "/images/images2.jpg"
            }
        };
        public IActionResult collectionofobjectpassing()
        {
            return View(emplist);
        }
        public IActionResult singleobjectpassing()
        {
            return View(obj);
        }
        public IActionResult display()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult sampledemo3()
        {
            int age = 34;
            string name = "ravi kumar";
            ViewBag.Name = name;
            ViewBag.Age = age;
            ViewData["Message"] = "Welcome to Asp .net core learning";
            ViewData["Year"] = DateTime.Now.Year;
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
