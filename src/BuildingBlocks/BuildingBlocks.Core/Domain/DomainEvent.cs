namespace BuildingBlocks.Core.Domain;

public record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccuredOn = DateTime.UtcNow;
    }
    public Guid Id { get; init; }
    public DateTime OccuredOn { get; init; }
}