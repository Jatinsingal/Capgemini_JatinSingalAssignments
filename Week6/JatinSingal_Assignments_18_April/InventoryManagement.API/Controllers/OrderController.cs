using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
                return NotFound("Customer not found");

            var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(x => productIds.Contains(x.ProductId))
                .ToDictionaryAsync(x => x.ProductId);

            if (products.Count != productIds.Count)
                return NotFound("One or more products were not found");

            foreach (var line in request.Items)
            {
                var product = products[line.ProductId];
                if (product.Quantity < line.Quantity)
                    return BadRequest($"Not enough stock for {product.Name}");
            }

            var totalAmount = request.Items.Sum(line =>
            {
                var product = products[line.ProductId];
                return product.Price * line.Quantity;
            });

            var order = new Order
            {
                CustomerId = request.CustomerId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItems = request.Items.Select(line =>
            {
                var product = products[line.ProductId];
                product.Quantity -= line.Quantity;

                return new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    Price = product.Price
                };
            }).ToList();

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order placed successfully",
                order = new
                {
                    order.OrderId,
                    order.CustomerId,
                    customerName = customer.Name,
                    order.OrderDate,
                    order.TotalAmount,
                    items = orderItems.Select(item => new
                    {
                        item.ProductId,
                        productName = products[item.ProductId].Name,
                        item.Quantity,
                        item.Price,
                        lineTotal = item.Price * item.Quantity
                    })
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var customers = await _context.Customers
                .ToDictionaryAsync(x => x.CustomerId, x => x.Name);
            var products = await _context.Products
                .ToDictionaryAsync(x => x.ProductId, x => x.Name);
            var orders = await _context.Orders
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
            var orderItems = await _context.OrderItems.ToListAsync();

            var data = orders.Select(order => new
            {
                order.OrderId,
                order.CustomerId,
                customerName = customers.GetValueOrDefault(order.CustomerId, "Unknown Customer"),
                order.OrderDate,
                order.TotalAmount,
                items = orderItems
                    .Where(item => item.OrderId == order.OrderId)
                    .Select(item => new
                    {
                        item.OrderItemId,
                        item.ProductId,
                        productName = products.GetValueOrDefault(item.ProductId, "Unknown Product"),
                        item.Quantity,
                        item.Price,
                        lineTotal = item.Price * item.Quantity
                    })
                    .ToList()
            });

            return Ok(data);
        }
    }
}
