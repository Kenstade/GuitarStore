using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.ShoppingCart.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.ShoppingCart.Models;

public sealed class CartItem : Entity<int>
{
    internal CartItem(Guid productId,string name, string image, int quantity, decimal price, Guid cartId)
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
    public string Image { get; private set; }
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

        builder.Property(x => x.Name)
            .HasColumnType("vrachar(50)");
        
        builder.Property(x => x.Image)
            .HasColumnType("varchar(255)");
        
        builder.HasKey(x => x.Id);
    }
}
