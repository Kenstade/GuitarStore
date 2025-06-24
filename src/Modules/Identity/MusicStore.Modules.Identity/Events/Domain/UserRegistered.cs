using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging.IntegrationEvents.Identity;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MusicStore.Modules.Identity.Events.Domain;

internal sealed record UserRegistered(Guid UserId, string Email) : DomainEvent;

internal sealed class UserRegisteredHandler(IBus bus, ILogger<UserRegisteredHandler> logger) 
    : IEventHandler<UserRegistered>
{
    public async Task Handle(UserRegistered domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling {Event}. EventId: {EventId}.", nameof(UserRegistered), domainEvent.Id);
        
        await bus.Publish(new UserRegisteredIntegrationEvent(domainEvent.UserId, domainEvent.Email), cancellationToken);
    }
}