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
        public IActionResult searchemp(int empid)
        {
            Employee emp = (from e1 in emplist where e1.EmployeeID == empid select e1).FirstOrDefault();
            return View(emp);
        }
        public string sampledemo2(int age, string name)
        {
            return "The name " + name + "having age" + age;
        }
        public IActionResult Details(int id)
        {
            var employee = emplist.FirstOrDefault(e => e.EmployeeID == id);
            if (employee == null) { 
                return NotFound();
            }
            return View(employee);
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
                ImageUrl = "/images/download.jpg",
                DeptId=10
            },
            new Employee
            {
                EmployeeID = 102,
                EmpName = "Meteor",
                Salary = 34000,
                ImageUrl = "/images/image1.jpg",
                DeptId = 20
            },
            new Employee
            {
                EmployeeID = 103,
                EmpName = "Bullet",
                Salary = 34000,
                ImageUrl = "/images/images.jpg",
                DeptId = 10
            },
            new Employee
            {
                EmployeeID = 104,
                EmpName = "Hunter",
                Salary = 34000,
                ImageUrl = "/images/images2.jpg",
                DeptId = 10
            }
        };
        public IActionResult collectionofdepts()
        {
            return View(deptlist);
        }
        public IActionResult empsindept(int deptid)
        {
            var employees = emplist.Where(e => e.DeptId == deptid).ToList();
            return View(employees);
        }
        public IActionResult mixedobjectpassing(int empid)
        {
            var query1 = deptlist.ToList();
            var emp = emplist.FirstOrDefault(x => x.EmployeeID == empid);

            if (emp == null)
            {
                return NotFound();
            }

            EmpdeptViewModel data = new EmpdeptViewModel
            {
                deptlist = query1,
                emp = emp,
                date = DateTime.Now
            };

            return View(data);
        }

        List<Dept> deptlist = new List<Dept>(){
         new Dept{DeptId=10,DeptName="Sales"},
         new Dept{DeptId=20,DeptName="HR"},
         new Dept{DeptId=30,DeptName="Software"}
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
