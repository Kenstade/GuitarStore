using GuitarStore.Common.Events;
using GuitarStore.Customers.Models;
using GuitarStore.Data;

namespace GuitarStore.Identity.Events;

public sealed record UserCreatedEvent(Guid Id, string Email) : INotification;
internal sealed class UserCreatedEventHandler(AppDbContext dbContext) : INotificationHandler<UserCreatedEvent>
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task Handle(UserCreatedEvent notification)
    {
        await _dbContext.Customers.AddAsync(new Customer
        {
            Id = notification.Id,
            Email = notification.Email
        });

        await _dbContext.SaveChangesAsync();
    }
}
