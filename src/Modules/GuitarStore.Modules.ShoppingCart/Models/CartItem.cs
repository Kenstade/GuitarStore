using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.ShoppingCart.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.ShoppingCart.Models;

public sealed class CartItem : Entity<int>
{
    internal CartItem(Guid productId, int quantity, decimal price, Guid cartId)
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
        CartId = cartId;
    }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public Guid CartId { get; private set; }

    internal void AddUnit()
    {
        Quantity++;
    }
}

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_item", CartDbContext.DefaultSchema);
        
        builder.HasKey(x => x.Id);
    }
}
