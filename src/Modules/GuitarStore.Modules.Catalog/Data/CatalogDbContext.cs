using BuildingBlocks.Core.Messaging;
using GuitarStore.Modules.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Catalog.Data;
public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "catalog";

    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SpecificationType> SpecificationTypes { get; set; }
    //TODO: сменить название?
    public DbSet<ProductSpecification> ProductSpecification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        modelBuilder.Entity<OutboxMessage>(); //TODO: сделать регистрацию автоматической

        base.OnModelCreating(modelBuilder);
    }
}
