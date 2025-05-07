using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Orders.Models;

internal sealed class OrderItem : Entity<int>
{
    internal OrderItem(Guid productId, string name, string image, decimal price, int quantity, Guid orderId)
    {
        ProductId = productId;
        Name = name;
        Image = image;
        Price = price;
        Quantity = quantity;
        OrderId = orderId;
    }
    
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public string? Image { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public Guid OrderId { get; private set; }
}

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .HasMaxLength(100);
    }
}
