using BuildingBlocks.Core.Events;

namespace BuildingBlocks.Core.Domain;
public interface IDomainEvent : IEvent
{ 
    Guid EventId => Guid.NewGuid();
    public DateTime OccuredOn => DateTime.UtcNow;
}
