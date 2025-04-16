using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;
internal sealed class ProductImage : Entity<int>
{
    internal ProductImage(string imageUrl, bool isMain, Guid productId)
    {
        ImageUrl = imageUrl;
        IsMain = isMain;
        ProductId = productId;
    }
    
    public string? ImageUrl { get; private set; }
    public bool IsMain { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
}

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_image", CatalogDbContext.DefaultSchema);
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ImageUrl)
            .HasColumnType("varchar(255)")
            .IsRequired();
    }
}
