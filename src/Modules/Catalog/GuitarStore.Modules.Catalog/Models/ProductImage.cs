using BuildingBlocks.Core.Domain;
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
        builder.ToTable("product_image");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ImageUrl)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId);
    }
}
