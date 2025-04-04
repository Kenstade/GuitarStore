using System.Collections.Concurrent;

namespace BuildingBlocks.Core.Events;

public sealed class EventPublisher : IEventPublisher
{
    private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();
    
    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<object>();
        }
        _handlers[eventType].Add(handler);
    }
    
    public async Task Publish<TEvent>(TEvent @event, CancellationToken ct = default) 
        where TEvent : IEvent
    {
        var eventType = @event.GetType();
        if (_handlers.ContainsKey(eventType))
        {
            foreach (IEventHandler<TEvent> handler in _handlers[eventType])
            {
                await handler.Handle(@event, ct);
            }
        }
    }
}