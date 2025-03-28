using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.Catalog.Models;

namespace GuitarStore.Modules.Catalog.Events;
public record ProductPriceChangedEvent(Product product) : IDomainEvent;
