using BuildingBlocks.Core.EFCore;
using GuitarStore.Modules.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Data;

internal sealed class CatalogDataSeeder(CatalogDbContext dbContext) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        await SeedCategoriesAsync();
        await SeedBrandsAsync();
        await SeedProductSpecificationTypes();
        await SeedProductsAsync();
        
        
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        if (!await dbContext.Categories.AnyAsync())
        {
            var categories = new List<Category>()
            {
                new("Electric guitars"),
                new("Acoustic guitars"),
                new("Guitar Amplifiers"),
            };

            await dbContext.Categories.AddRangeAsync(categories);
        }
    }

    private async Task SeedBrandsAsync()
    {
        if (!await dbContext.Brands.AnyAsync())
        {
            var brands = new List<Brand>()
            {
                new("FENDER"),
                new("IBANEZ"),
                new("EART"),
                new("YAMAHA"),
                new("VESTON"),
                new("VOX"),
                new("Orange"),
            };

            await dbContext.Brands.AddRangeAsync(brands);
        }
    }

    private async Task SeedProductSpecificationTypes()
    {
        if (!await dbContext.SpecificationTypes.AnyAsync())
        {
            var types = new List<SpecificationType>
            {
                new("Strings", 1),
                new("Frets", 1),
                new("PickupConfiguration", 1),
                new("Fingerboard", 1),
                new("Body", 1),
                new("Neck", 1),
                new("Top", 1),
                new("Bridge", 1)
            };

            await dbContext.SpecificationTypes.AddRangeAsync(types);
        }
    }

    private async Task SeedProductsAsync()
    {
        if (!await dbContext.Products.AnyAsync())
        {
            var products = new List<Product>()
            {
                Product.Create("IBANEZ GRGR131EX-BKF", "description", 319.99M, ProductColor.Unspecified, 10, 1, 2),
                Product.Create("IBANEZ GRX70QA-TRB", "description", 309.99M,ProductColor.Unspecified, 10, 1, 2),
                Product.Create("IBANEZ GRGR221PA-AQB", "description", 409.95M,ProductColor.Unspecified, 10, 1, 2),
                Product.Create("IBANEZ GIO GRG170DX BKN", "description", 419.99M,ProductColor.Unspecified, 10, 1, 2),
                Product.Create("FENDER SQUIER Affinity 2021 Stratocaster MN Black", "description", 469.99M,ProductColor.Unspecified, 10, 1, 1),
                Product.Create("FENDER PLAYER Telecaster MN Butterscotch Blonde", "description", 1299.99M,ProductColor.Unspecified, 10, 1, 1),
                Product.Create("EART GW2 Natural", "description", 469.95M,ProductColor.Unspecified, 10, 1, 3),
                
                Product.Create("YAMAHA F310", "description", 239.99M,ProductColor.Unspecified, 10, 2, 4),
                Product.Create("VESTON F-38/BK", "description", 99.95M,ProductColor.Unspecified, 10, 2, 5),
                Product.Create("FENDER CD-60 Black", "description", 269.99M,ProductColor.Unspecified, 10, 2, 1),
                Product.Create("IBANEZ PF1512-NT", "description", 329.99M,ProductColor.Unspecified, 10, 2, 2),
                Product.Create("IBANEZ AC340-OPN", "description", 359.99M,ProductColor.Unspecified, 10, 2, 2),
                Product.Create("IBANEZ PF15-BK", "description", 299.99M,ProductColor.Unspecified, 10, 2, 2),
                Product.Create("FENDER FA-125", "description", 229.99M,ProductColor.Unspecified, 10, 2, 1),
                
                Product.Create("VOX VT20X", "description", 329.99M,ProductColor.Unspecified, 10, 3, 6),
                Product.Create("VOX VX15-GT", "description", 299.99M,ProductColor.Unspecified, 10, 3, 6),
                Product.Create("IBANEZ IBZ10BV2", "description", 139.99M,ProductColor.Unspecified, 10, 3, 2),
                Product.Create("IBANEZ T15II", "description", 169.99M,ProductColor.Unspecified, 10, 3, 2),
                Product.Create("Orange Crush 20RT", "description", 249.99M,ProductColor.Unspecified, 10, 3, 7)
            };

            await dbContext.Products.AddRangeAsync(products);
        }
    }

    //private async Task SeedProductSpecifications()
    //{
    //}

}
