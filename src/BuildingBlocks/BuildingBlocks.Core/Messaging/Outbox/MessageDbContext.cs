using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Messaging.Outbox;

public sealed class MessageDbContext(DbContextOptions<MessageDbContext> options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureOutboxMessage();
        base.OnModelCreating(modelBuilder);
    }
}
