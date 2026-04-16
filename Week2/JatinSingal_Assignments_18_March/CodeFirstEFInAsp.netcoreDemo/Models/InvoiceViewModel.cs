namespace CodeFirstEFInAsp.netcoreDemo.Models
{
    public class InvoiceViewModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime InvoiceDate { get; set; }

        public Customer Customer { get; set; } = null!;

        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount => Quantity * UnitPrice;
    }
}
