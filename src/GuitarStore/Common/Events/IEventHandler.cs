namespace GuitarStore.Common.Events;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task Handle(TEvent message);
}
