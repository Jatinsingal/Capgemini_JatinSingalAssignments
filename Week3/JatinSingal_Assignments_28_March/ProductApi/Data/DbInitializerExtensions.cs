using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data;

public static class DbInitializerExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();

            if (await context.Products.AnyAsync())
            {
                return;
            }

            context.Products.AddRange(
                new Product { Name = "Laptop", Price = 65000M, Category = "Electronics" },
                new Product { Name = "Desk Chair", Price = 7500M, Category = "Furniture" },
                new Product { Name = "Notebook", Price = 120M, Category = "Stationery" });

            await context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Database initialization was skipped. Verify the SQL Server instance and connection string. Details: {exception.Message}");
        }
    }
}