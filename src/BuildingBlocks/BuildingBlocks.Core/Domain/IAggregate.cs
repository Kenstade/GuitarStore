namespace BuildingBlocks.Core.Domain;

public interface IAggregate : IEntity
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
