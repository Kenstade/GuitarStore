using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.ShoppingCart.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.ShoppingCart.Models;

public sealed class Cart : Aggregate<Guid>
{
    private readonly List<CartItem> _items = [];
    public Guid CustomerId { get; private set; }
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public static Cart Create(Guid customerId)
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId
        };
        
        return cart;
    }
    
    internal void AddItem(Guid productId,string name, string image, decimal price)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem == null)
        {
            _items.Add(new CartItem(productId, name, image, 1, price, Id));
            return;
        }
        
        existingItem.AddUnit();
    }

    internal void RemoveItem(Guid productId)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        
        if(existingItem != null) _items.Remove(existingItem);
    }
}

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("cart", CartDbContext.DefaultSchema);
        
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId);
    }
}
