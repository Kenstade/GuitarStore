namespace BuildingBlocks.Core.Domain;
public interface IDomainEvent
{ 
    Guid EventId => Guid.NewGuid();
    public DateTime OccuredOn => DateTime.UtcNow;
}
