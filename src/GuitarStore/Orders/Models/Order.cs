using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Orders.Models;

public class Order
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime AddedAt { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
    public Address Address { get; set; } = null!;

}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId);

        builder.OwnsOne(o => o.Address, x =>
        {
            x.Property(a => a.City)
                .HasColumnName("City");

            x.Property(a => a.Street)
                .HasColumnName("Street");

            x.Property(a => a.BuildingNumber)
                .HasColumnName("BuildingNumber");

            x.Property(a => a.Apartment)
                .HasColumnName("RoomNumber");
        });
    }
}
