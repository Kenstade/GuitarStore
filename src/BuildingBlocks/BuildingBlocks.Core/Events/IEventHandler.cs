using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public interface IEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent, CancellationToken ct = default);
}