﻿using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.Catalog.Data;
using GuitarStore.Modules.Catalog.Events;
using GuitarStore.Modules.Catalog.Exceptions;
using GuitarStore.Modules.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

//https://www.youtube.com/watch?v=MJcaxi3bOO8&t=426s strongly typed ids
public sealed class Product : Aggregate<ProductId>
{
    private readonly List<ProductSpecification> _specifications = [];
    private readonly List<ProductImage> _images = [];

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public ProductColor Color { get; private set; }
    public int Stock { get; private set; }
    public bool IsAvailable { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public int BrandId { get; private set; } 
    public Brand Brand { get; private set; } = null!;
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<ProductSpecification> Specifications => _specifications.AsReadOnly();


    private Product() { }
    
    public static Product Create(
        string name, 
        string description, 
        decimal price, 
        ProductColor color,
        int stock,        
        int categoryId, 
        int brandId,
        IList<ProductImage>? images,
        bool isAvailable = true)
    {
        var product = new Product
        {
            Id = ProductId.NewProductId(),
            Name = name,
            Description = description,
            Price = price,
            Color = color,
            Stock = stock,
            CreatedAt = DateTime.UtcNow,
            IsAvailable = isAvailable,
            CategoryId = categoryId,
            BrandId = brandId,
        };

        product.AddImages(images);
        product.AddDomainEvent(new ProductCreatedEvent(2455555));
        return product;
    }

    public void Update(string name,
        string description,
        decimal price,
        ProductColor color,
        int stock,
        int categoryId,
        int brandId,
        IList<ProductImage>? images,
        bool isAvailable = true)
    {
        Name = name;
        Description = description;
        Color = color;
        Stock = stock;
        IsAvailable = isAvailable;
        CategoryId = categoryId;
        BrandId = brandId;
        AddImages(images);

        if(Price != price)
        {
            Price = price;
            AddDomainEvent(new ProductPriceChangedEvent(this));
        }
    }

    public void AddImages(IList<ProductImage>? images)
    {
        if (images is null) return;

        _images.AddRange(images);
    }

    public int RemoveStock(int quantity)
    {
        if (Stock < quantity)
        {
            throw new InsufficientStockException(
                $"Empty stock, product item '{Name}' with quantity {quantity} is not available.");
        }
        int removed = Math.Min(quantity, Stock);

        Stock -= removed;
        //TODO: ?
        if(Stock == 0) IsAvailable = false;
        
        return removed;
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", CatalogDbContext.DefaultSchema);

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(pId => pId.Value, value => new(value))
            .ValueGeneratedNever();

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);

        builder.HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId);

        //TODO: разобраться с many-to-many
        builder.HasMany(p => p.Specifications)
            .WithOne(ps => ps.Product)
            .HasForeignKey(ps => ps.ProductId);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId);

        builder.Property(p => p.Color)
            .HasDefaultValue(ProductColor.Unspecified)
            .HasMaxLength(25)
            .HasConversion(p => p.ToString(), p => Enum.Parse<ProductColor>(p));
    }
}