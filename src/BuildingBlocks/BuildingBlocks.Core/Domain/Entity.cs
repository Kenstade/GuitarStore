namespace BuildingBlocks.Core.Domain;

public abstract class Entity<TId> : IEntity
{
    public TId Id { get; protected set; } = default!;
    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
public abstract class Entity : IEntity
{
    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
