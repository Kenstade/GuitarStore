using BuildingBlocks.Core.Domain;

namespace GuitarStore.Modules.Catalog.Events;

public sealed record PriceChanged(Guid ProductId, decimal Price) : IDomainEvent;