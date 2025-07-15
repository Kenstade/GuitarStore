using BuildingBlocks.Core.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Customer.Models;

namespace MusicStore.Modules.Customer.Data;
internal sealed class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "customers";
    
    public DbSet<Models.Customer> Customers => Set<Models.Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
