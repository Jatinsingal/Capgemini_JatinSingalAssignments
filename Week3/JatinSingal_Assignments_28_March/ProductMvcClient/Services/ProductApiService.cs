using System.Net;
using System.Net.Http.Json;
using ProductMvcClient.Models;
using ProductMvcClient.Services.Interfaces;

namespace ProductMvcClient.Services;

public class ProductApiService : IProductApiService
{
    private readonly HttpClient _httpClient;

    public ProductApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductViewModel>> GetAllProductsAsync()
    {
        var response = await _httpClient.GetAsync("api/products");
        await EnsureSuccessAsync(response);
        var products = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
        return products ?? new List<ProductViewModel>();
    }

    public async Task<ProductViewModel?> GetProductByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/products/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProductViewModel>();
    }

    public async Task CreateProductAsync(ProductViewModel product)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products", product);
        await EnsureSuccessAsync(response);
    }

    public async Task UpdateProductAsync(ProductViewModel product)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/products/{product.Id}", product);
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}");
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync();
        throw new ApplicationException(string.IsNullOrWhiteSpace(details)
            ? $"API request failed with status code {(int)response.StatusCode}."
            : details);
    }
}