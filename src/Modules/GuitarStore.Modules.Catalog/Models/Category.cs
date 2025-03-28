using GuitarStore.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<SpecificationType> SpecificationTypes { get; set; } = default!;
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories", CatalogDbContext.DefaultSchema);
        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.SpecificationTypes)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId);
    }
}
