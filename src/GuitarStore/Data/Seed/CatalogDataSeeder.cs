using GuitarStore.Catalogs.Models;
using GuitarStore.Data.Interfaces;

namespace GuitarStore.Data.Seed;

public class CatalogDataSeeder(AppDbContext dbContext) : IDataSeeder
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task SeedAllAsync()
    {
        await SeedCategoriesAsync();
        await SeedProductsAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        if (!_dbContext.Categories.Any())
        {
            var categories = new List<Category>()
            {
                new() { Name = "Electric guitars" },
                new() { Name = "Acoustic guitars"},
            };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedProductsAsync()
    {
        if (!_dbContext.Products.Any())
        {
            var categories = new List<Product>()
            {
                new() { Name = "YAMAHA F310", Description = "text", Image = "url", Price = 239.99M, Stock = 10, IsAvailable = true, CategoryId = 2 },
                new() { Name = "VESTON F-38/BK", Description = "text", Image = "url", Price = 99.95M, Stock = 10, IsAvailable = true, CategoryId = 2},
                new() { Name = "Schecter SGR AVENGER M BLK", Description = "text", Image = "url", Price = 299.50M, Stock = 10, IsAvailable = true, CategoryId = 1},
                new() { Name = "IBANEZ GRGR131EX-BKF", Description = "text", Image = "url", Price = 319.99M, Stock = 10, IsAvailable = true, CategoryId = 1},
                new() { Name = "IBANEZ GRX70QA-TRB", Description = "text", Image = "url", Price = 309.99M, Stock = 10, IsAvailable = true, CategoryId = 1}
            };

            await _dbContext.Products.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();
        }
    }
}
