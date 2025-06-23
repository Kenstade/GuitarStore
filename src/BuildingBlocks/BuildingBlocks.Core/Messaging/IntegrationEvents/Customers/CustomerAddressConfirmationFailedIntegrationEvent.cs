namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Customers;

public sealed record CustomerAddressConfirmationFailedIntegrationEvent(Guid CorrelationId, Guid OrderId) 
    : IntegrationEvent;