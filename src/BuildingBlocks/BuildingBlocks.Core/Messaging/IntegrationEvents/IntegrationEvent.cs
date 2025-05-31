namespace BuildingBlocks.Core.Messaging.IntegrationEvents;

public record IntegrationEvent
{
    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccuredOn = DateTime.UtcNow;
    }
    
    public Guid Id { get; init; }
    public DateTime OccuredOn { get; init; }
}