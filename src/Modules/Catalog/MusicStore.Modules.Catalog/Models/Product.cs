using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicStore.Modules.Catalog.Models;

internal sealed class Product : Aggregate<Guid>
{
    private readonly List<ProductSpecification> _specifications = [];
    private readonly List<ProductImage> _images = [];

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public ProductColor Color { get; private set; }
    public int AvailableStock { get; private set; }
    public int? ReservedStock { get; private set; }
    public bool IsAvailable { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public int BrandId { get; private set; } 
    public Brand Brand { get; private set; } = null!;
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<ProductSpecification> Specifications => _specifications.AsReadOnly();
    
    public static Product Create(
        string name, 
        string description, 
        decimal price, 
        ProductColor color,
        int stock,        
        int categoryId, 
        int brandId,
        IList<ProductImage>? images = null,
        bool isAvailable = true)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Color = color,
            AvailableStock = stock,
            CreatedAt = DateTime.UtcNow,
            IsAvailable = isAvailable,
            CategoryId = categoryId,
            BrandId = brandId,
        };

        product.AddImages(images);
        
        return product;
    }

    public void AddImages(IList<ProductImage>? images)
    {
        if (images is null) return;

        _images.AddRange(images);
    }

    public string? GetMainImage() => _images.FirstOrDefault(p => p.IsMain)?.ImageUrl;

    public bool HasAvailableStock(int quantity)
    {
        if (quantity < 0) quantity *= -1;
        
        return AvailableStock >= quantity;
    }

    public void ReserveStock(int quantity)
    {
        if (quantity < 0) quantity *= -1;
        
        RemoveStock(quantity);
        
        ReservedStock += quantity;
    }
    
    public void RemoveStock(int quantity)
    {
        AvailableStock -= quantity;
        
        if(AvailableStock == 0)
            Deactivate();
    }

    public void RestoreStock(int quantity)
    {
        if (quantity < 0) quantity *= -1;
        AvailableStock += quantity;
    }
    
    public void Activate() => IsAvailable = true;
    public void Deactivate() => IsAvailable = false;
}

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products")
            .HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(p => p.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);

        builder.HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId);

        builder.Property(p => p.Color)
            .HasDefaultValue(ProductColor.Unspecified)
            .HasMaxLength(25)
            .HasConversion(p => p.ToString(), p => Enum.Parse<ProductColor>(p));
    }
}