namespace CodeFirstEFInAsp.netcoreDemo.Models
{
    public class Author1
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public IList<Course1> Courses { set; get; }

    }
}
