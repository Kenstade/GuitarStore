namespace BuildingBlocks.Core.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, CancellationToken ct = default, TimeSpan? expiration = null);
    Task RemoveAsync(string key, CancellationToken ct = default);
}