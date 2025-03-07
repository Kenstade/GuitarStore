using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.ShoppingCart.Models;

public class Cart
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<CartItem> Items { get; set; } = [];

    internal void AddItem(int productId, decimal price)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem == null)
        {
            Items.Add(new CartItem { ProductId = productId, Quantity = 1, Price = price});
            return;
        }
        existingItem.Quantity++;
    }
}

internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId);
    }
}
