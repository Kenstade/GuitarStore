namespace BuildingBlocks.Core.EFCore;

public sealed class PostgresOptions
{
    public string ConnectionString { get; init; } = null!;
    public bool UseInMemory { get; init; }
}
