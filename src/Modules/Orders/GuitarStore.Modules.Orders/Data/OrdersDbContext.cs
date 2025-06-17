using BuildingBlocks.Core.Messaging.Outbox;
using GuitarStore.Modules.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Orders.Data;
internal sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "orders";
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
