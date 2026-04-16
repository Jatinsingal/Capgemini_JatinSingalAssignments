namespace MVCExampleDemo.Models
{
    public class Dept
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }

        List<Employee> Employees { get; set; } = new List<Employee>();

    }
}
