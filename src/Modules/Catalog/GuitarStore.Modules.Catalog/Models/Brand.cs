using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

internal sealed class Brand : Entity<int>
{
    public Brand(string name)
    {
        Name = name;
    }
    
    public string Name { get; private set; }
}

internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");
        
        builder.HasKey(x => x.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(100);
    }
}
