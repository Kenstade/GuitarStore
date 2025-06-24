using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Catalog.Models;

namespace MusicStore.Modules.Catalog.Data;
internal sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "catalog";

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SpecificationType> SpecificationTypes => Set<SpecificationType>();
    public DbSet<ProductSpecification> ProductSpecification => Set<ProductSpecification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
