namespace BuildingBlocks.Core.Caching;

public sealed class RedisOptions
{
    public string ConnectionString { get; set; } = default!;
    public bool UseInMemory { get; set; }
}