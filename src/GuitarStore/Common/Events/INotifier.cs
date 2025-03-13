namespace GuitarStore.Common.Events;

public interface INotifier
{
    Task Send<TEvent>(TEvent message) where TEvent : IEvent;
}
