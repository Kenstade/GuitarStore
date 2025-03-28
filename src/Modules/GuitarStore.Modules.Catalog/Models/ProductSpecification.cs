using GuitarStore.Modules.Catalog.Data;
using GuitarStore.Modules.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

public sealed class ProductSpecification
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public int SpecificationTypeId { get; set; }
    public SpecificationType SpecificationType { get; set; } = default!;
    public ProductId ProductId { get; set; }
    public Product Product { get; set; } = default!;
}

public sealed class ProductSpecificationConfiguration : IEntityTypeConfiguration<ProductSpecification>
{
    public void Configure(EntityTypeBuilder<ProductSpecification> builder)
    {
        builder.ToTable("product_specifications", CatalogDbContext.DefaultSchema);
    }
}
