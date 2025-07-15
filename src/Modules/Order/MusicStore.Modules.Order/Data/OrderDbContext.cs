using BuildingBlocks.Core.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Order.Models;

namespace MusicStore.Modules.Order.Data;
internal sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "orders";
    
    public DbSet<Models.Order> Orders => Set<Models.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
