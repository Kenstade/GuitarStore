using GuitarStore.Catalogs.Models;

namespace GuitarStore.Data;

public static class AppDbContextSeed
{
    // переделать и добавить логер
    public static async Task SeedDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await SeedCategoriesAsync(db);
        await SeedProductsAsync(db);
    }
    private static async Task SeedCategoriesAsync(AppDbContext db)
    {
        if (!db.Categories.Any())
        {
            var categories = new List<Category>()
            {
                new() { Name = "Electric guitars" },
                new() { Name = "Acoustic guitars"},
            };

            await db.Categories.AddRangeAsync(categories);
            await db.SaveChangesAsync();
        }
    }
    private static async Task SeedProductsAsync(AppDbContext db)
    {
        if (!db.Products.Any())
        {
            var categories = new List<Product>()
            {
                new() { Name = "YAMAHA F310", Description = "text", Image = "url", Price = 239.99M, Stock = 10, IsAvailable = true, CategoryId = 2 },
                new() { Name = "VESTON F-38/BK", Description = "text", Image = "url", Price = 99.95M, Stock = 10, IsAvailable = true, CategoryId = 2},
                new() { Name = "Schecter SGR AVENGER M BLK", Description = "text", Image = "url", Price = 299.50M, Stock = 10, IsAvailable = true, CategoryId = 1},
                new() { Name = "IBANEZ GRGR131EX-BKF", Description = "text", Image = "url", Price = 319.99M, Stock = 10, IsAvailable = true, CategoryId = 1},
                new() { Name = "IBANEZ GRX70QA-TRB", Description = "text", Image = "url", Price = 309.99M, Stock = 10, IsAvailable = true, CategoryId = 1}
            };

            await db.Products.AddRangeAsync(categories);
            await db.SaveChangesAsync();
        }
    }
}
