using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

internal sealed class ProductSpecification : Entity<int>
{
    public string Value { get; private set; } = string.Empty;
    public int SpecificationTypeId { get; private set; }
    public SpecificationType SpecificationType { get; private set; } = default!;
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
}

internal sealed class ProductSpecificationConfiguration : IEntityTypeConfiguration<ProductSpecification>
{
    public void Configure(EntityTypeBuilder<ProductSpecification> builder)
    {
        builder.ToTable("product_specification");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .HasColumnType("varchar(50)");
    }
}
