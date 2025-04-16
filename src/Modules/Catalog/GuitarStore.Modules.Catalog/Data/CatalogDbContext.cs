using BuildingBlocks.Core.Messaging.Outbox;
using GuitarStore.Modules.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Data;
internal sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "catalog";

    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SpecificationType> SpecificationTypes { get; set; }
    // сменить название?
    public DbSet<ProductSpecification> ProductSpecification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureOutboxMessage();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
