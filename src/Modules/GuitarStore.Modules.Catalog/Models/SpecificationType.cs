﻿using GuitarStore.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Catalog.Models;

public class SpecificationType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<ProductSpecification> ProductSpecifications { get; set; } = default!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}

public class SpecificationTypeConfiguration : IEntityTypeConfiguration<SpecificationType>
{
    public void Configure(EntityTypeBuilder<SpecificationType> builder)
    {
        builder.ToTable("specification_type", CatalogDbContext.DefaultSchema);

        builder.HasMany(s => s.ProductSpecifications)
            .WithOne(ps => ps.SpecificationType)
            .HasForeignKey(ps => ps.SpecificationTypeId);
    }
}
