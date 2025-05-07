using GuitarStore.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

internal sealed class SpecificationType
{
    internal SpecificationType(string name, int categoryId)
    {
        Name = name;
        CategoryId = categoryId;
    }
    
    public int Id { get; private set; }
    public string Name { get; private set; }
    public ICollection<ProductSpecification> ProductSpecifications { get; private set; } = default!;
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;
}

internal sealed class SpecificationTypeConfiguration : IEntityTypeConfiguration<SpecificationType>
{
    public void Configure(EntityTypeBuilder<SpecificationType> builder)
    {
        builder.ToTable("specification_type");
        
        builder.HasKey(x => x.Id);

        builder.Property(s => s.Name)
            .HasColumnType("varchar(50)");

        builder.HasMany(s => s.ProductSpecifications)
            .WithOne(ps => ps.SpecificationType)
            .HasForeignKey(ps => ps.SpecificationTypeId);
    }
}
