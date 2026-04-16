using Microsoft.AspNetCore.Mvc;
using UniversityCourseApp.Models;

namespace UniversityCourseApp.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly List<Student> _students;
        private readonly List<Course> _courses;

        public EnrollmentController()
        {
            _courses = new()
            {
                new() { CourseId = 1, Title = "Data Structures", Credits = 4, Department = "CSE" },
                new() { CourseId = 2, Title = "Algorithms", Credits = 4, Department = "CSE" },
                new() { CourseId = 3, Title = "Databases", Credits = 3, Department = "CSE" },
                new() { CourseId = 4, Title = "Web Dev", Credits = 3, Department = "IT" },
                new() { CourseId = 5, Title = "OS", Credits = 4, Department = "CSE" }
            };

            _students = new()
            {
                new() { StudentId = 1, Name = "Alice", Branch = "CSE", Enrollments = new()
                {
                    new() { CourseId = 1, Grade = "A", AttemptNumber = 1 },
                    new() { CourseId = 2, Grade = "A-", AttemptNumber = 1 },
                    new() { CourseId = 3, Grade = "B+", AttemptNumber = 1 }
                }},

                new() { StudentId = 2, Name = "Bob", Branch = "CSE", Enrollments = new()
                {
                    new() { CourseId = 1, Grade = "B", AttemptNumber = 1 },
                    new() { CourseId = 4, Grade = "A", AttemptNumber = 1 },
                    new() { CourseId = 5, Grade = "B+", AttemptNumber = 1 }
                }},

                new() { StudentId = 3, Name = "Charlie", Branch = "IT", Enrollments = new()
                {
                    new() { CourseId = 4, Grade = "C", AttemptNumber = 1 },
                    new() { CourseId = 1, Grade = "B-", AttemptNumber = 1 }
                }},

                new() { StudentId = 4, Name = "Diana", Branch = "CSE", Enrollments = new()
                {
                    new() { CourseId = 2, Grade = "A", AttemptNumber = 1 },
                    new() { CourseId = 5, Grade = "F", AttemptNumber = 1 },
                    new() { CourseId = 5, Grade = "B", AttemptNumber = 2 }
                }},

                new() { StudentId = 5, Name = "Eve", Branch = "IT", Enrollments = new() }
            };
        }

        public IActionResult Index()
        {
            var studentCourses = _students
                .Select(s => new StudentCoursesVM
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Branch = s.Branch,

                    CourseTitles = s.Enrollments
                        .GroupBy(e => e.CourseId)
                        .Select(g => g.OrderByDescending(e => e.AttemptNumber).First())
                        .Join(_courses,
                              e => e.CourseId,
                              c => c.CourseId,
                              (e, c) => c.Title)
                        .ToList()
                })
                .ToList();

            return View(studentCourses);
        }

        public IActionResult Details(int studentId)
        {
            var student = _students.FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                return NotFound();

            var studentDetails = new StudentDetailVM
            {
                Name = student.Name,
                Branch = student.Branch,

                Courses = student.Enrollments
                    .GroupBy(e => e.CourseId)
                    .Select(g => g.OrderByDescending(e => e.AttemptNumber).First())
                    .Join(_courses,
                        e => e.CourseId,
                        c => c.CourseId,
                        (e, c) => new CourseDetailVM
                        {
                            Title = c.Title,
                            Credits = c.Credits,
                            Department = c.Department,
                            LatestGrade = e.Grade
                        })
                    .ToList()
            };

            return View(studentDetails);
        }
    }
}
