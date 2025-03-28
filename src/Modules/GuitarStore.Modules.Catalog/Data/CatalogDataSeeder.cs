﻿using BuildingBlocks.Core.EFCore;
using GuitarStore.Modules.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Data;

internal sealed class CatalogDataSeeder(CatalogDbContext dbContext) : IDataSeeder
{
    private readonly CatalogDbContext _dbContext = dbContext;
    public async Task SeedAllAsync()
    {
        await SeedCategoriesAsync();
        await SeedBrandsAsync();
        await SeedProductSpecificationTypes();
        await SeedProductsAsync();
        //await SeedProductSpecifications();
    }

    private async Task SeedCategoriesAsync()
    {
        if (!await _dbContext.Categories.AnyAsync())
        {
            var categories = new List<Category>()
            {
                new() { Name = "Electric guitars"},
                new() { Name = "Acoustic guitars"},
                new() { Name = "Guitar Amplifiers"},
            };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedBrandsAsync()
    {
        if (!await _dbContext.Brands.AnyAsync())
        {
            var brands = new List<Brand>()
            {
                new() { Name = "IBANEZ"},
                new() { Name = "FENDER"},
                new() { Name = "EART"},
                new() { Name = "YAMAHA"},
                new() { Name = "VESTON"},
                new() { Name = "VOX"},
                new() { Name = "Orange "},
            };

            await _dbContext.Brands.AddRangeAsync(brands);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedProductSpecificationTypes()
    {
        if (!await _dbContext.SpecificationTypes.AnyAsync())
        {
            var types = new List<SpecificationType>
            {
                new() { Name = "Strings", CategoryId = 1 },
                new() { Name = "Frets", CategoryId = 1 },
                new() { Name = "PickupConfiguration", CategoryId = 1 },
                new() { Name = "Fingerboard", CategoryId = 1 },
                new() { Name = "Body", CategoryId = 1 },
                new() { Name = "Neck", CategoryId = 1 },
                new() { Name = "Top", CategoryId = 1 },
                new() { Name = "Bridge", CategoryId = 1 },

            };

            await _dbContext.SpecificationTypes.AddRangeAsync(types);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedProductsAsync()
    {
        if (!await _dbContext.Products.AnyAsync())
        {
            var products = new List<Product>()
            {
                Product.Create("IBANEZ GRGR131EX-BKF", "text", 319.99M, ProductColor.Unspecified, 10, 1, 2, null),
                //new() { Name = "IBANEZ GRGR131EX-BKF", Description = "text", Image = "url", Price = 319.99M, Stock = 10, IsAvailable = true, CategoryId = 1, BrandId = 2},
                //new() { Name = "IBANEZ GRX70QA-TRB", Description = "text", Image = "url", Price = 309.99M, Stock = 10, IsAvailable = true, CategoryId = 1, BrandId = 2},
                //new() { Name = "IBANEZ GRGR221PA-AQB", Description = "text", Image = "url", Price = 409.95M, Stock = 10, IsAvailable = true, CategoryId = 1, BrandId = 2},
                //new() { Name = "IBANEZ GIO GRG170DX BKN", Description = "text", Image = "url", Price = 419.99M, Stock = 10, IsAvailable = true, CategoryId = 1, BrandId = 2},
                //new() { Name = "FENDER SQUIER Affinity 2021 Stratocaster MN Black", Description = "text", Image = "url", Price = 469.99M, Stock = 10, IsAvailable = true, CategoryId = 1, BrandId = 3},
                //new() { Name = "FENDER PLAYER Telecaster MN Butterscotch Blonde", Description = "text", Image = "url", Price = 1299.99M, Stock = 10, IsAvailable = true, CategoryId = 1, BrandId = 3},
                //new() { Name = "EART GW2 Natural", Description = "text", Image = "url", Price = 469.95M, Stock = 10, IsAvailable = true, CategoryId = 1, BrandId = 4},               

                //new() { Name = "YAMAHA F310", Description = "text", Image = "url", Price = 239.99M, Stock = 10, IsAvailable = true, CategoryId = 2, BrandId = 5 },
                //new() { Name = "VESTON F-38/BK", Description = "text", Image = "url", Price = 99.95M, Stock = 10, IsAvailable = true, CategoryId = 2, BrandId = 6},
                //new() { Name = "FENDER CD-60 Black", Description = "text", Image = "url", Price = 269.99M, Stock = 10, IsAvailable = true, CategoryId = 2, BrandId = 3},
                //new() { Name = "IBANEZ PF1512-NT", Description = "text", Image = "url", Price = 329.99M, Stock = 10, IsAvailable = true, CategoryId = 2, BrandId = 2},
                //new() { Name = "IBANEZ AC340-OPN", Description = "text", Image = "url", Price = 359.99M, Stock = 10, IsAvailable = true, CategoryId = 2, BrandId = 2},
                //new() { Name = "IBANEZ PF15-BK", Description = "text", Image = "url", Price = 299.99M, Stock = 10, IsAvailable = true, CategoryId = 2, BrandId = 2},
                //new() { Name = "FENDER FA-125", Description = "text", Image = "url", Price = 229.99M, Stock = 10, IsAvailable = true, CategoryId = 2, BrandId = 3},

                //new() { Name = "VOX VT20X", Description = "text", Image = "url", Price = 329.99M, Stock = 10, IsAvailable = true, CategoryId = 3, BrandId = 7 },
                //new() { Name = "VOX VX15-GT", Description = "text", Image = "url", Price = 299.99M, Stock = 10, IsAvailable = true, CategoryId = 3, BrandId = 7 },
                //new() { Name = "IBANEZ IBZ10BV2", Description = "text", Image = "url", Price = 139.99M, Stock = 10, IsAvailable = true, CategoryId = 3, BrandId = 2 },
                //new() { Name = "IBANEZ T15II", Description = "text", Image = "url", Price = 169.99M, Stock = 10, IsAvailable = true, CategoryId = 3, BrandId = 2 },
                //new() { Name = "Orange Crush 20RT", Description = "text", Image = "url", Price = 249.99M, Stock = 10, IsAvailable = true, CategoryId = 3, BrandId = 8 },
            };

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
        }
    }

    //private async Task SeedProductSpecifications()
    //{
    //    if (!await _dbContext.ProductSpecification.AnyAsync())
    //    {
    //        var specs = new List<ProductSpecification>
    //        {
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 1 },
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 2 },
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 3 },
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 4 },
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 5 },
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 6 },
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 7 },
    //            new() { Value = "6", SpecificationTypeId = 1, ProductId = 8 },

    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 1 },
    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 2 },
    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 3 },
    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 4 },
    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 5 },
    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 6 },
    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 7 },
    //            new() { Value = "22", SpecificationTypeId = 2, ProductId = 8 },

    //        };

    //        await _dbContext.ProductSpecification.AddRangeAsync(specs);
    //        await _dbContext.SaveChangesAsync();
    //    }
    //}

}
