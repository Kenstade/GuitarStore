namespace GuitarStore.Common.Events;

public interface INotificationHandler<T> where T : INotification
{
    Task Handle(T notification);
}
