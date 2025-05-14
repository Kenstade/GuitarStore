namespace BuildingBlocks.Core.EFCore;

public class PostgresOptions
{
    public string ConnectionString { get; init; } = null!;
    public bool UseInMemory { get; init; }
}
