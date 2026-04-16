namespace UniversityCourseApp.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;

        public List<Enrollment> Enrollments { get; set; } = new();
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Department { get; set; } = string.Empty;

        public List<Enrollment> Enrollments { get; set; } = new();
    }

    public class Enrollment
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string Grade { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
    }
}