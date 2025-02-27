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