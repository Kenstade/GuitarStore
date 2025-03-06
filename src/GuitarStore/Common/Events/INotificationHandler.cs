namespace GuitarStore.Common.Events;

public interface INotificationHandler<TMessage> where TMessage : INotification
{
    Task Handle(TMessage message);
}
