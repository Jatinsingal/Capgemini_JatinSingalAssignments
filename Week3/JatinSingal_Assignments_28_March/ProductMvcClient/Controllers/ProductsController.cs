using Microsoft.AspNetCore.Mvc;
using ProductMvcClient.Models;
using ProductMvcClient.Services.Interfaces;

namespace ProductMvcClient.Controllers;

public class ProductsController : Controller
{
    private readonly IProductApiService _productApiService;
    private readonly IConfiguration _configuration;

    public ProductsController(IProductApiService productApiService, IConfiguration configuration)
    {
        _productApiService = productApiService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5058/";
        try
        {
            var products = await _productApiService.GetAllProductsAsync();
            return View(products);
        }
        catch (Exception exception)
        {
            ViewBag.ErrorMessage = exception.Message;
            return View(new List<ProductViewModel>());
        }
    }

    public IActionResult Create()
    {
        return View(new ProductViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        try
        {
            await _productApiService.CreateProductAsync(product);
            TempData["SuccessMessage"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(product);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var product = await _productApiService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }
        catch (Exception exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        try
        {
            await _productApiService.UpdateProductAsync(product);
            TempData["SuccessMessage"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(product);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var product = await _productApiService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }
        catch (Exception exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _productApiService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}