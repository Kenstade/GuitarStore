using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Messaging;

public sealed class MessageDbContext(DbContextOptions<MessageDbContext> options) : DbContext(options)
{
    //public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessageDbContext).Assembly);
        modelBuilder.Entity<OutboxMessage>();
        base.OnModelCreating(modelBuilder);
    }
}
