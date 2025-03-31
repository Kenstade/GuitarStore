using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

public class Brand : Entity<int>
{
    internal Brand(string name)
    {
        Name = name;
    }
    public string Name { get; private set; }
}

public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands", CatalogDbContext.DefaultSchema);
        
        builder.HasKey(x => x.Id);

        builder.Property(b => b.Name)
            .HasColumnType("varchar(100)");
    }
}
