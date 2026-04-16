using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Services.Interfaces;

namespace ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception exception)
        {
            return DatabaseUnavailable(exception);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        catch (Exception exception)
        {
            return DatabaseUnavailable(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct(Product product)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var createdProduct = await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }
        catch (Exception exception)
        {
            return DatabaseUnavailable(exception);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest("Product id mismatch.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var updated = await _productService.UpdateProductAsync(product);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception exception)
        {
            return DatabaseUnavailable(exception);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception exception)
        {
            return DatabaseUnavailable(exception);
        }
    }

    private ObjectResult DatabaseUnavailable(Exception exception)
    {
        return StatusCode(StatusCodes.Status503ServiceUnavailable,
            $"Database connection attention needed. Verify VISHAL\\SQLEXPRESS and SQL Server encryption settings. Details: {exception.Message}");
    }
}