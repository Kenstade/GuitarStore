using MediatR;

namespace BuildingBlocks.Core.Domain;
public interface IDomainEvent : INotification
{ 
    Guid EventId => Guid.NewGuid();
    public DateTime OccuredOn => DateTime.UtcNow;
}
