using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Orders.Models;

internal sealed class Order : Aggregate<Guid>
{
    private readonly List<OrderItem> _items = [];
    
    public Guid CustomerId { get; private set; }
    public Guid AddressId { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(Guid customerId, Guid addressId)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            AddressId = addressId,
            OrderStatus = OrderStatus.Placed
        };
        
        order.AddDomainEvent(new OrderStatusChangedToPlaced(order.Id, order.CustomerId));
        
        return order;
    }

}

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);

        builder.Property(p => p.OrderStatus)
            .HasMaxLength(25)
            .HasConversion(p => p.ToString(), p => Enum.Parse<OrderStatus>(p));
    }
}
