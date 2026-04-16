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
    }
}