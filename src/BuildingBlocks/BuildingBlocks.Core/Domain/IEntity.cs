namespace BuildingBlocks.Core.Domain;
public interface IEntity<TId> : IEntity
{
    public TId Id { get; set; }
}

public interface IEntity
{
    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
