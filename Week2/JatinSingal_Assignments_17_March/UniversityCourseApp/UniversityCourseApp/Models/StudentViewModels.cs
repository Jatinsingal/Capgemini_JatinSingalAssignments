namespace UniversityCourseApp.Models
{
    public class StudentCoursesVM
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;

        public List<string> CourseTitles { get; set; } = new();

        public int CourseCount => CourseTitles.Count;
    }

    public class CourseDetailVM
    {
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Department { get; set; } = string.Empty;
        public string LatestGrade { get; set; } = string.Empty;
    }

    public class StudentDetailVM
    {
        public string Name { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;

        public List<CourseDetailVM> Courses { get; set; } = new();
    }
}
