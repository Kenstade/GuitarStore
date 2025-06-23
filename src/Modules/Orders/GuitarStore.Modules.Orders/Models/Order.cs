using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.Orders.Events.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Orders.Models;

internal sealed class Order : Aggregate<Guid>
{
    private readonly List<OrderItem> _items = [];
    
    public Guid CustomerId { get; private set; }
    public Guid AddressId { get; private set; }
    public Address Address { get; private set; } = null!;
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
        
        order.AddDomainEvent(new OrderPlaced(Guid.NewGuid(), order.Id, order.CustomerId));
        
        return order;
    }

}

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders")
            .HasKey(o => o.Id);
        
        builder.OwnsOne(o => o.Address, x =>
        {
            x.Property(c => c.City)
                .HasMaxLength(100);

            x.Property(c => c.Street)
                .HasMaxLength(100);

            x.Property(c => c.BuildingNumber)
                .HasMaxLength(50);

            x.Property(c => c.Apartment)
                .HasMaxLength(100);
        });

        builder.Property(p => p.OrderStatus)
            .HasMaxLength(25)
            .HasConversion(p => p.ToString(), p => Enum.Parse<OrderStatus>(p));
    }
}
