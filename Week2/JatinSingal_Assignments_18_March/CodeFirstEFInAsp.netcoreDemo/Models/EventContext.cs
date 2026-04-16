using Microsoft.EntityFrameworkCore;

namespace CodeFirstEFInAsp.netcoreDemo.Models
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options)
            : base(options)
        {
        }

        public DbSet<Author> authors { get; set; }
        public DbSet<Course> courses { get; set; }
        public DbSet<Student> students { get; set; }
        public DbSet<Course1> course1 { get; set; }
        public DbSet<Author1> author1 { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<UserDetail> userDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
