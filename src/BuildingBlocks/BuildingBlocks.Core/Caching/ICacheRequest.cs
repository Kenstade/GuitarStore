﻿namespace BuildingBlocks.Core.Caching;

public interface ICacheRequest
{
    string CacheKey { get; }
}
