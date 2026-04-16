using CodeFirstEFInAsp.netcoreDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstEFInAsp.netcoreDemo.Controllers
{
    public class TransactionController : Controller
    {
        private readonly EventContext _context;

        public TransactionController(EventContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View(new PurchaseViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCustomer(PurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = new Customer
            {
                CustomerName = model.CustomerName
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            var product = new Product
            {
                ProductName = model.ProductName,
                CustomerID = customer.CustomerID
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Invoice), new
            {
                customerId = customer.CustomerID,
                productId = product.ProductID,
                quantity = model.Quantity,
                unitPrice = model.UnitPrice
            });
        }

        [HttpGet]
        public IActionResult Invoice(int customerId, int productId, int quantity, decimal unitPrice)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerID == customerId);
            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);

            if (customer == null || product == null)
            {
                return NotFound();
            }

            var invoice = new InvoiceViewModel
            {
                InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{customer.CustomerID}{product.ProductID}",
                InvoiceDate = DateTime.Now,
                Customer = customer,
                Product = product,
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            return View(invoice);
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(CreateCustomer));
        }
    }
}
