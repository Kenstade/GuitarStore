using GuitarStore.Catalogs.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Catalogs.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductColor Color { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAvailable { get; set; }
    public ICollection<ProductSpecification> ProductSpecifications { get; set; } = default!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = default!;

    internal int RemoveStock(int quantity)
    {
        if (Stock < quantity)
        {
            throw new InsufficientStockException(
                $"Empty stock, product item '{Name}' with quantity {quantity} is not available.");
        }
        int removed = Math.Min(quantity, Stock);

        Stock -= removed;
        //нужно ли делать товар недоступным при нулевом стоке?
        if(Stock == 0) IsAvailable = false;
        
        return removed;
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);

        builder.HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId);

        builder.HasMany(p => p.ProductSpecifications)
            .WithOne(ps => ps.Product)
            .HasForeignKey(ps => ps.ProductId);


        builder.Property(p => p.Color)
            .HasDefaultValue(ProductColor.Unspecified)
            .HasMaxLength(25)
            .HasConversion(p => p.ToString(), p => Enum.Parse<ProductColor>(p));
    }
}