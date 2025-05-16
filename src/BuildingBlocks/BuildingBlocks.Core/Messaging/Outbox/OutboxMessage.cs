using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Core.Messaging.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = null!;
    public string Content { get; init; } = null!;
    public DateTime OccuredOn { get; init; }
    public DateTime? ProcessedOn { get; set; }
    public string? Error { get; set; } 
}

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage> 
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type)
            .HasMaxLength(100);
        
        builder.Property(m => m.Content)
            .HasMaxLength(2000)
            .HasColumnType("jsonb");
    }
}
