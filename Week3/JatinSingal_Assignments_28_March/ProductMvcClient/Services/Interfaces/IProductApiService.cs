using ProductMvcClient.Models;

namespace ProductMvcClient.Services.Interfaces;

public interface IProductApiService
{
    Task<List<ProductViewModel>> GetAllProductsAsync();
    Task<ProductViewModel?> GetProductByIdAsync(int id);
    Task CreateProductAsync(ProductViewModel product);
    Task UpdateProductAsync(ProductViewModel product);
    Task DeleteProductAsync(int id);
}