namespace BuildingBlocks.Core.Messaging.IntegrationEvents.Customers;

public sealed record CustomerAddressConfirmedIntegrationEvent(
    Guid CorrelationId,
    Guid OrderId,
    string City, 
    string Street,
    string BuildingNumber,
    string Apartment) 
    : IntegrationEvent;