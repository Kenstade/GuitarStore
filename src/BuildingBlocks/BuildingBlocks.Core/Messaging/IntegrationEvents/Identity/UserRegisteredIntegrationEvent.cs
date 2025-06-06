namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Identity;

public sealed record UserRegisteredIntegrationEvent(Guid UserId, string Email) : IntegrationEvent;