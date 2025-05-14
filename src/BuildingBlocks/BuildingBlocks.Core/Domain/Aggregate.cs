namespace BuildingBlocks.Core.Domain;
public abstract class Aggregate<TId> : Entity<TId>, IAggregate
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
