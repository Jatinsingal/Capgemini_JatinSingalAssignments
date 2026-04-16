using System.ComponentModel.DataAnnotations;

namespace ProductMvcClient.Models;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter product name")]
    [StringLength(100)]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Please enter product price")]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Please enter product category")]
    [StringLength(80)]
    public string? Category { get; set; }
}