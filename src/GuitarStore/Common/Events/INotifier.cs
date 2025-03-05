namespace GuitarStore.Common.Events;

public interface INotifier
{
    Task Send<T>(T Notification) where T : INotification;
}
