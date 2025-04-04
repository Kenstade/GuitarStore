using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public interface IEventPublisher
{
    void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent;
    Task Publish<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent;
}