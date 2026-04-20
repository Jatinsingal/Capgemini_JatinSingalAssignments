using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.API.DTOs
{
    public class PlaceOrderRequest
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [MinLength(1)]
        public List<OrderLineRequest> Items { get; set; } = new();
    }

    public class OrderLineRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
