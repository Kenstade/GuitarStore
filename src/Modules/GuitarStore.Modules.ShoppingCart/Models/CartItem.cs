using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.ShoppingCart.Models;

public sealed class CartItem : Entity<int>
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public Guid CartId { get; private set; }

    internal static CartItem Create(Guid productId, decimal price, Guid cartId)
    {
        var cartItem = new CartItem
        {
            ProductId = productId,
            Price = price,
            CartId = cartId
        };
        cartItem.AddUnit();
        
        return cartItem;
    }

    internal void AddUnit()
    {
        Quantity++;
    }
}

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
