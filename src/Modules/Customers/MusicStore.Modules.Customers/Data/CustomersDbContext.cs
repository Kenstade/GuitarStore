using BuildingBlocks.Core.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Customers.Models;

namespace MusicStore.Modules.Customers.Data;
internal sealed class CustomersDbContext(DbContextOptions<CustomersDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "customers";
    
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
