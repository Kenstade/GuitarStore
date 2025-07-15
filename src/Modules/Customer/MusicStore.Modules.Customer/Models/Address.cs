using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicStore.Modules.Customer.Models;

internal sealed class Address : Entity<Guid>
{
    public Address(string city, string street, string buildingNumber, string apartment, Guid customerId)
    {
        City = city;
        Street = street;
        BuildingNumber = buildingNumber;
        Apartment = apartment;
        CustomerId = customerId;
    }
    
    public string City { get; set; }
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
    public string Apartment { get; set; }
    public Guid CustomerId { get; set; }

    public void Update(string? city, string? street, string? buildingNumber, string? apartment)
    {
        City = city ?? City;
        Street = street ?? Street;
        BuildingNumber = buildingNumber ?? BuildingNumber;
        Apartment = apartment ?? Apartment;
    }
}

internal sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses")
            .HasKey(c => c.Id);
        
        builder.HasOne<Customer>()
            .WithMany(c => c.Addresses)
            .HasForeignKey(c => c.CustomerId);

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
