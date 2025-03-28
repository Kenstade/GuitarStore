namespace BuildingBlocks.Core.Domain;
public interface IAggregate<TId> : IAggregate, IEntity<TId>
{
}

public interface IAggregate : IEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
