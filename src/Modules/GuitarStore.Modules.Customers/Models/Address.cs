﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Customers.Models;

public class Address
{
    public Guid Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string BuildingNumber { get; set; } = string.Empty;
    public string Apartment { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

internal sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(c => c.City)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(c => c.Street)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(c => c.BuildingNumber)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(c => c.Apartment)
            .IsRequired()
            .HasColumnType("varchar(100)");
    }
}
