using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

internal sealed class SpecificationType : Entity<int>
{
    private readonly List<ProductSpecification> _productSpecifications = [];
    internal SpecificationType(string name, int categoryId)
    {
        Name = name;
        CategoryId = categoryId;
    }
    
    public string Name { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public IReadOnlyCollection<ProductSpecification> Specifications => _productSpecifications.AsReadOnly();
}

internal sealed class SpecificationTypeConfiguration : IEntityTypeConfiguration<SpecificationType>
{
    public void Configure(EntityTypeBuilder<SpecificationType> builder)
    {
        builder.ToTable("specification_type");
        
        builder.HasKey(x => x.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(50);
        
        builder.HasOne(st => st.Category)
            .WithMany(c => c.SpecificationTypes)
            .HasForeignKey(st => st.CategoryId);
    }
}
