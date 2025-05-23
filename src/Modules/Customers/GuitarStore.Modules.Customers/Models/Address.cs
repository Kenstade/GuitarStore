using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Customers.Models;

internal sealed class Address : Entity<Guid>
{
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string BuildingNumber { get; set; } = null!;
    public string Apartment { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

internal sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");
        builder.HasKey(x => x.Id);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Street)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.BuildingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Apartment)
            .IsRequired()
            .HasMaxLength(100);
    }
}
