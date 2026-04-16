using System.ComponentModel.DataAnnotations;

namespace CodeFirstEFInAsp.netcoreDemo.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please enter the product name.")]
        public string ProductName { get; set; }

        public int CustomerID { get; set; }

        public Customer Customer { get; set; } = null!;
    }
}
