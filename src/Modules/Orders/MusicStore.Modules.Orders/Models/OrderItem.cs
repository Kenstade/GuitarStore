using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicStore.Modules.Orders.Models;

internal sealed class OrderItem : Entity<int>
{
    public OrderItem(Guid productId, string name, string? image, decimal price, int quantity, Guid orderId)
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

    public void AddUnits(int quantity)
    {
        Quantity += quantity;
    }
}

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(i => i.Id);
        
        builder.HasOne<Order>()
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId);

        builder.Property(i => i.Name)
            .HasMaxLength(100);

        builder.Property(i => i.Image)
            .HasMaxLength(255); 
    }
}
