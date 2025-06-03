using FCG.Core.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FCG.Infra.Cache;

public class MemCacheService(IMemoryCache cache) : ICacheService
{
    private readonly IMemoryCache _cache = cache;
    public object? Get(string key) => _cache.TryGetValue(key, out var cachedValue) ? cachedValue : null;
    public void Remove(string key) => _cache.Remove(key);
    public void Set(string key, object content) => _cache.Set(key, content, TimeSpan.FromMinutes(10));
}