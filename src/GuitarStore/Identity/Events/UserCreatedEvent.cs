using GuitarStore.Common.Events;
using GuitarStore.Customers.Models;
using GuitarStore.Data;

namespace GuitarStore.Identity.Events;

public sealed record UserCreatedEvent(Guid Id, string Email) : INotification;

internal sealed class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserCreatedEventHandler> _logger;
    public UserCreatedEventHandler(AppDbContext dbContext, ILogger<UserCreatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(UserCreatedEvent notification)
    {
        _logger.LogInformation($"Handling {nameof(UserCreatedEvent)}");

        await _dbContext.Customers.AddAsync(new Customer
        {
            Id = notification.Id,
            Email = notification.Email
        });

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"{nameof(UserCreatedEvent)} completed");
    }
}
