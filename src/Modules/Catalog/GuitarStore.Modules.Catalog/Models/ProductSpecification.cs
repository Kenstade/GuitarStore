using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

internal sealed class ProductSpecification : Entity
{
    public ProductSpecification(string value, int specificationTypeId, Guid productId)
    {
        Value = value;
        SpecificationTypeId = specificationTypeId;
        ProductId = productId;
    }
    
    public string Value { get; private set; }
    public int SpecificationTypeId { get; private set; }
    public SpecificationType SpecificationType { get; private set; } = null!;
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
}

internal sealed class ProductSpecificationConfiguration : IEntityTypeConfiguration<ProductSpecification>
{
    public void Configure(EntityTypeBuilder<ProductSpecification> builder)
    {
        builder.ToTable("product_specification");
        
        builder.HasKey(ps => new { ps.SpecificationTypeId, ps.ProductId });
        
        builder.Property(x => x.Value)
            .HasMaxLength(50);
        
        builder.HasOne<Product>()
            .WithMany(p => p.Specifications)
            .HasForeignKey(ps => ps.ProductId);
        
        builder.HasOne<SpecificationType>()
            .WithMany(sp => sp.Specifications)
            .HasForeignKey(sp => sp.SpecificationTypeId);
    }
}
