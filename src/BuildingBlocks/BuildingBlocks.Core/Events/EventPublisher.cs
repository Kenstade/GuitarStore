using System.Collections.Concurrent;
using BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Events;

public sealed class EventPublisher(IServiceProvider serviceProvider, ILogger<EventPublisher> logger) : IEventPublisher
{
    private static readonly ConcurrentDictionary<Type, EventHandlerWrapper> _eventHandlers = [];

    public Task Publish<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IDomainEvent
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }
        
        return PublishEvent(@event, ct);
    }

    private static async Task ForeachAwaitPublisher(IEnumerable<EventHandlerExecutor> handlerExecutors, IDomainEvent @event, CancellationToken ct)
    {
        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(@event, ct).ConfigureAwait(false);
        }
    }
    
    private Task PublishEvent(IDomainEvent @event, CancellationToken ct)
    {
        var handler = _eventHandlers.GetOrAdd(@event.GetType(), static eventType =>
        {
            var wrapperType = typeof(EventHandlerWrapperImpl<>).MakeGenericType(eventType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException();

            return (EventHandlerWrapper)wrapper;
        });
        
        return handler.Handle(@event, serviceProvider, ForeachAwaitPublisher, ct);
    }
    
}