using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public interface IEventPublisher
{
    Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken ct = default) where TDomainEvent : IDomainEvent;
}