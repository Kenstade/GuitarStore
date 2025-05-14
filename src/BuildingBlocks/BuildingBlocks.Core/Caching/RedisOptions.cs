namespace BuildingBlocks.Core.Caching;

public sealed class RedisOptions
{
    public string ConnectionString { get; init; } = null!;
    public bool UseInMemory { get; init; }
}