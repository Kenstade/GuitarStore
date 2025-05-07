using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Messaging.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Content { get; set; }
    public DateTime OccuredOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public string? Error { get; set; } 
}

public static class OutboxModelBuilderExtensions 
{
    public static void ConfigureOutboxMessage(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(m => m.Id);
        
            builder.Property(m => m.Content)
                .HasColumnType("jsonb");
        });
    }
}
