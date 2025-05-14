using BuildingBlocks.Core.Messaging.Outbox;
using GuitarStore.Modules.Customers.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Customers.Data;
internal sealed class CustomersDbContext(DbContextOptions<CustomersDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customer");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
        modelBuilder.ConfigureOutboxMessage();
        base.OnModelCreating(modelBuilder);
    }
}
