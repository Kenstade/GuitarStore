using BuildingBlocks.Core.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Events;

public abstract class EventHandlerWrapper
{
    public abstract Task Handle(IDomainEvent @event, IServiceProvider serviceProvider,
        Func<IEnumerable<EventHandlerExecutor>, IDomainEvent, CancellationToken, Task> publish,
        CancellationToken cancellationToken);
}

public sealed class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper where TEvent : IDomainEvent
{
    public override Task Handle(IDomainEvent @event, IServiceProvider serviceProvider, 
        Func<IEnumerable<EventHandlerExecutor>, IDomainEvent, CancellationToken, Task> publish, 
        CancellationToken cancellationToken)
    {
        var handlers = serviceProvider
            .GetServices<IEventHandler<TEvent>>()
            .Select(static handler => new EventHandlerExecutor(handler, (@event,ct) => handler.Handle((TEvent)@event, ct)));
        
        return publish(handlers, @event, cancellationToken);
    }
}

public sealed record EventHandlerExecutor(object HandlerInstance, Func<IDomainEvent, CancellationToken, Task> HandlerCallback);