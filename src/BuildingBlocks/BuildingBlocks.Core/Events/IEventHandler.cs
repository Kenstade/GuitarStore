using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent @event, CancellationToken ct = default);
}