using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.ShoppingCart.Models;

internal sealed class CartItem : Entity<int>
{
    internal CartItem(Guid productId, string name, string? image, int quantity, decimal price, Guid cartId)
    {
        ProductId = productId;
        Name = name;
        Image = image;
        Quantity = quantity;
        Price = price;
        CartId = cartId;
    }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public string? Image { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public Guid CartId { get; private set; }

    internal void AddUnit()
    {
        Quantity++;
    }
}

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");
        builder.HasKey(x => x.Id);
        
        builder.HasOne<Cart>()
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.CartId);
        
        builder.Property(x => x.Name)
            .HasMaxLength(50);
        
        builder.Property(x => x.Image)
            .HasMaxLength(255);
    }
}
