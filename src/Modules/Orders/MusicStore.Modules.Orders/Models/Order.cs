using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Modules.Orders.Events.Domain;

namespace MusicStore.Modules.Orders.Models;

internal sealed class Order : Aggregate<Guid>
{
    private readonly List<OrderItem> _items = [];
    
    public Guid CustomerId { get; private set; }
    public Guid AddressId { get; private set; }
    public Address? Address { get; private set; }
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

    public void AddItem(Guid productId, string name, string? image, decimal price, int quantity, Guid orderId)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.AddUnits(quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId, name, image, price, quantity, orderId));
        }
        
        Total += price * quantity;
    }

    public void AddAddress(Guid correlationId, string city, string street, string buildingNumber, string apartment)
    {
        Address = new Address(city, street, buildingNumber, apartment);
        
        AddDomainEvent(new CustomerAddressAddedToOrder(correlationId));
    }

    public void SetAwaitingValidationStatus(Guid correlationId)
    {
        OrderStatus = OrderStatus.AwaitingValidation;
        AddDomainEvent(new OrderStatusChangedToAwaitingValidation(correlationId, Id));
    }

    public void SetAwaitingPaymentStatus()
    {
        OrderStatus = OrderStatus.AwaitingPayment;
    }

    public void SetCancelledStatus()
    {
        OrderStatus = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelled(Guid.NewGuid(), Id));
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
