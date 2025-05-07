using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

internal sealed class Category : Entity<int>
{
    internal Category(string name)
    {
        Name = name;
    }
    public string Name { get; private set; }
    public ICollection<SpecificationType> SpecificationTypes { get; set; } = default!;
}

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasColumnType("varchar(50)");

        builder.HasMany(c => c.SpecificationTypes)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId);
    }
}
