using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Orders.Models;

internal sealed class Order : Aggregate<Guid>
{
    private readonly List<OrderItem> _items = [];
    
    public Guid CustomerId { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Address Address { get; private set; } = null!;

    internal static Order Create(Guid customerId, decimal total, OrderStatus orderStatus, Address address)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Total = total,
            OrderStatus = orderStatus,
            Address = address,
        };
        
        return order;
    }

}

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId);

        builder.OwnsOne(o => o.Address, x =>
        {
            x.Property(a => a.City)
                .HasMaxLength(50)
                .HasColumnName("city");

            x.Property(a => a.Street)
                .HasMaxLength(100)
                .HasColumnName("street");

            x.Property(a => a.BuildingNumber)
                .HasMaxLength(50)
                .HasColumnName("building_number");

            x.Property(a => a.Apartment)
                .HasMaxLength(50)
                .HasColumnName("room_numer");
        });

        builder.Property(p => p.OrderStatus)
            .HasMaxLength(25)
            .HasConversion(p => p.ToString(), p => Enum.Parse<OrderStatus>(p));
    }
}
