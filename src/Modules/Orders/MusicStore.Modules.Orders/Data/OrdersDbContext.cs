using BuildingBlocks.Core.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Orders.Models;

namespace MusicStore.Modules.Orders.Data;
internal sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "orders";
    
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
