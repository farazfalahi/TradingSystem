using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace TradingSystem.Infrastructure.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        _cache.Set(key, value, ttl ?? TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    public Task<T> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T value);
        return Task.FromResult(value);
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}