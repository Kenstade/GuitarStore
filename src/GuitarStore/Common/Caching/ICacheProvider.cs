﻿namespace GuitarStore.Common.Caching;

public interface ICacheProvider
{
    Task<TResponse> GetOrCreateAsync<TResponse>(ICacheRequest request, Func<Task<TResponse>> factory);
}
