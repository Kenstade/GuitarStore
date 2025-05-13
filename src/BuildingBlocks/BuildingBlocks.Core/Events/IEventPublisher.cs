using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public interface IEventPublisher
{
    Task Publish<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IDomainEvent;
}