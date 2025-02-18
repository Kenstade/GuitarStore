using GuitarStore.Customers.Models;
using GuitarStore.Data;

namespace GuitarStore.Identity.Events;

public sealed record UserCreatedEvent(Guid Id, string Email);
internal sealed class UserEventHandler(UserCreatedEventHandler userCreatedEventHandler)
{
    private readonly UserCreatedEventHandler _userCreatedEventHandler = userCreatedEventHandler;

    public Task Handle(UserCreatedEvent userCreatedEvent)
    {
        return _userCreatedEventHandler.Hanlde(userCreatedEvent);
    }
}

internal sealed class UserCreatedEventHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserCreatedEvent> _logger;
    public UserCreatedEventHandler(AppDbContext dbContext, ILogger<UserCreatedEvent> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Hanlde(UserCreatedEvent userCreatedEvent)
    {
        _logger.LogInformation($"Handling {nameof(UserCreatedEvent)}");

        await _dbContext.Customers.AddAsync(new Customer 
        { 
            Id = userCreatedEvent.Id,
            Email = userCreatedEvent.Email 
        });

        await _dbContext.SaveChangesAsync();
    }
}
