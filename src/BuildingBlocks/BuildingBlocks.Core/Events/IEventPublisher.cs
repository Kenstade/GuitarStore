using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public interface IEventPublisher
{
    Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent;
}