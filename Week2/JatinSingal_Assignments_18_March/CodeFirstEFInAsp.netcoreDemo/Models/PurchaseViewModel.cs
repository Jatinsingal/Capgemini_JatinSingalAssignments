using System.ComponentModel.DataAnnotations;

namespace CodeFirstEFInAsp.netcoreDemo.Models
{
    public class PurchaseViewModel
    {
        [Required(ErrorMessage = "Please enter the customer name.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please enter the product name.")]
        public string ProductName { get; set; }

        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000.")]
        public int Quantity { get; set; } = 1;

        [Range(typeof(decimal), "1", "1000000", ErrorMessage = "Unit price must be greater than 0.")]
        public decimal UnitPrice { get; set; }
    }
}
