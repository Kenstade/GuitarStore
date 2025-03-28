
namespace BuildingBlocks.Core.Domain;
public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
