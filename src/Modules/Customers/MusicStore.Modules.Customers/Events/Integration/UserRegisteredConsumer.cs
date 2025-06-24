using BuildingBlocks.Core.Messaging.IntegrationEvents.Identity;
using MassTransit;
using Microsoft.Extensions.Logging;
using MusicStore.Modules.Customers.Data;
using MusicStore.Modules.Customers.Models;

namespace MusicStore.Modules.Customers.Events.Integration;

internal sealed class UserRegisteredConsumer(CustomersDbContext dbContext, ILogger<UserRegisteredConsumer> logger) 
    : IConsumer<UserRegisteredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}.", 
            nameof(UserRegisteredIntegrationEvent), context.Message.Id);
        
        var customer = Customer.Create(context.Message.UserId, context.Message.Email);
        
        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("Customer '{@UserId}' created.", context.Message.UserId);
    }
}