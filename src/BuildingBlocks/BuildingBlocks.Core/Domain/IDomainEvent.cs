namespace BuildingBlocks.Core.Domain;
public interface IDomainEvent
{ 
    Guid Id => Guid.NewGuid();
    public DateTime OccuredOn => DateTime.UtcNow;
}
