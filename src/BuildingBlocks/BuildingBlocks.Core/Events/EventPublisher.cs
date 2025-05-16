using System.Collections.Concurrent;
using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.Core.Events;

public sealed class EventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    private static readonly ConcurrentDictionary<Type, EventHandlerWrapper> EventHandlers = [];

    public Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken ct = default) 
        where TDomainEvent : IDomainEvent
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));
        
        return PublishEvent(domainEvent, ct);
    }

    private static async Task ForeachAwaitPublisher(IEnumerable<EventHandlerExecutor> handlerExecutors, 
        IDomainEvent domainEvent, CancellationToken ct)
    {
        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(domainEvent, ct).ConfigureAwait(false);
        }
    }
    
    private Task PublishEvent(IDomainEvent domainEvent, CancellationToken ct)
    {
        var handler = EventHandlers.GetOrAdd(domainEvent.GetType(), static eventType =>
        {
            var wrapperType = typeof(EventHandlerWrapperImpl<>).MakeGenericType(eventType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException();

            return (EventHandlerWrapper)wrapper;
        });
        
        return handler.Handle(domainEvent, serviceProvider, ForeachAwaitPublisher, ct);
    }
    
}