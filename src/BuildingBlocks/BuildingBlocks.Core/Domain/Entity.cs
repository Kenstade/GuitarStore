namespace BuildingBlocks.Core.Domain;

public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
public abstract class Entity : IEntity
{
    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
