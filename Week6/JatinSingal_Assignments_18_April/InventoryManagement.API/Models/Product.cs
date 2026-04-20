using System.ComponentModel.DataAnnotations;

public class Product
{
    public int ProductId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(1, 1000000)]
    public decimal Price { get; set; }

    [Range(0, 10000)]
    public int Quantity { get; set; }
}