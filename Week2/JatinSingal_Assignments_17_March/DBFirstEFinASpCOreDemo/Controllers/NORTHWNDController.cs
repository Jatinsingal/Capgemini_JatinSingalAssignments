using DBFirstEFinASpCOreDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DBFirstEFinASpCOreDemo.Controllers
{
    public class NORTHWNDController : Controller
    {
        public IActionResult SpainCustomers()
        {
            var cnt = new NORTHWNDContext();

            var spainscustomer = cnt.Customers
                                    .Where(x => x.Country == "Spain")
                                    .Select(x => new CustomerVM
                                    {
                                        CustomerId = x.CustomerId,
                                        ContactName = x.ContactName,
                                        CompanyName = x.CompanyName
                                    })
                                    .ToList();

            return View(spainscustomer);
        }

        public IActionResult searchCutomer(string contactname)
        {
            var cnt = new NORTHWNDContext();
            var searchcustomer = cnt.Customers
                            .Where(x => x.ContactName == contactname)
                            .Select(x => new Customer
                            {
                                ContactName = x.ContactName,
                                ContactTitle = x.ContactTitle,
                                CompanyName = x.CompanyName
                            })
                            .Single();

            return View(searchcustomer);
        }
        public IActionResult ProductsInCategory(string? categoryname)
        {
            var cnt = new NORTHWNDContext();

            if (string.IsNullOrEmpty(categoryname))
            {
                // No search yet → return empty list
                return View(new List<ProdCat>());
            }

            var productsinCategory = cnt.Products
                .Where(p => p.Category != null && p.Category.CategoryName == categoryname)
                .Select(p => new ProdCat
                {
                    prodname = p.ProductName,
                    catname = p.Category.CategoryName
                })
                .ToList();

            return View(productsinCategory);
        }

        public ActionResult OrderRange(string range)
        {
            var cnt = new NORTHWNDContext();
            var range1 = Convert.ToInt16(range);
            var custOrderCount = cnt.Customers.Where(x => x.Orders.Count > range1).Select(x => new Customer
            {
                CustomerId = x.CustomerId,
                ContactName = x.ContactName
            });
            return View(custOrderCount);
        }
    }
}