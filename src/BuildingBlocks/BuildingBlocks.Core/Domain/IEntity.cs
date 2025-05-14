namespace BuildingBlocks.Core.Domain;
public interface IEntity
{
    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
