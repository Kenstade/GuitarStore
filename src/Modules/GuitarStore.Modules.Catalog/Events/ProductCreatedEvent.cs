using BuildingBlocks.Core.Domain;

namespace GuitarStore.Modules.Catalog.Events;
public record ProductCreatedEvent(int biba) : IDomainEvent;
