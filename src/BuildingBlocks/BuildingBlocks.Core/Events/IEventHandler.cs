using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task Handle(TEvent @event, CancellationToken ct);
}