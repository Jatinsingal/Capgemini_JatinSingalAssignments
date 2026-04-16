using Microsoft.AspNetCore.Mvc;
using RoutingCGExample.Models;

namespace RoutingCGExample.Controllers
{
    public class StudentController : Controller
    {
        List<Student> studlist = new List<Student>()
        {
            new Student{Id=101, Name="Kiran", Class="class4"},
            new Student{Id=102, Name="Mohan", Class="class7"},
            new Student{Id=103, Name="suhana", Class="class8"}
        };
        [Route("studs")]
        public IActionResult GetAllStudents()
        {
            return View(studlist);
        }
        [Route("studs/{id}")]
        public IActionResult GetAllStudent(int id)
        {
            var student = studlist.FirstOrDefault(s => s.Id == id);
            return View();
        }


        public IActionResult GetStudents(int id)
        {
            var student = studlist.FirstOrDefault(s => s.Id == id);
            return View(student);
        }
        [Route("studs/fewColumns")]
        public IActionResult fewcolumns()
        {
            var fewcolumns = studlist
                .Select(s => new Student
                {
                    Class = s.Class,
                    Name = s.Name
                })
                .ToList();

            return View(fewcolumns);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
