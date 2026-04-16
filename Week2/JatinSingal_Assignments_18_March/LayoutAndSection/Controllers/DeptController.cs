using LayoutAndSection.Models;
using Microsoft.AspNetCore.Mvc;

namespace LayoutAndSection.Controllers
{
    public class DeptController : Controller
    {
        List<Department> deptlist = new List<Department>()
            {
                new Department{DeptID=10,DeptName="Sales"},
                new Department{DeptID=20,DeptName="HR"},
                new Department{DeptID=30,DeptName="Software"}
            };

        public IActionResult Index()
        {
            return View(deptlist);
        }
    }
}
