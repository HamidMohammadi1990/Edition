using Edition.Domain.Caching.Enums;
using Edition.Domain.Caching.Abstractions;

namespace Edition.Infrastructure.CacheProviders;

public class DistributedCache
    (IRedisCache redis, IDatabaseSelector databaseSelector) 
    : ICache
{    
    public T? Get<T>(string key, CacheInstanceType instanceTypeCode)
      => redis.Get<T>(key, databaseSelector.Select(instanceTypeCode));

    public string? Get(string key, CacheInstanceType instanceTypeCode)
      => redis.Get(key, databaseSelector.Select(instanceTypeCode));

    public async Task<T?> GetAsync<T>(string key, CacheInstanceType instanceTypeCode, CancellationToken token = default)
      => await redis.GetAsync<T>(key, databaseSelector.Select(instanceTypeCode), token);

    public async Task<string?> GetAsync(string key, CacheInstanceType instanceTypeCode, CancellationToken token = default)
      => await redis.GetAsync<string>(key, databaseSelector.Select(instanceTypeCode), token);

    public void Refresh(string key) => redis.Refresh(key);

    public async Task RefreshAsync(string key, CancellationToken token = default) => await redis.RefreshAsync(key, token);

    public bool Remove(string key, CacheInstanceType instanceTypeCode)
      => redis.Remove(key, databaseSelector.Select(instanceTypeCode));

    public async Task<bool> RemoveAsync(string key, CacheInstanceType instanceTypeCode, CancellationToken token = default)
      => await redis.RemoveAsync(key, databaseSelector.Select(instanceTypeCode), token);

    public bool Set<T>(string key, T value, int duration, CacheInstanceType instanceTypeCode, bool extend = false) where T : class
      => redis.Set(key, value, duration, databaseSelector.Select(instanceTypeCode), extend);

    public bool Set<T>(string key, T value, TimeSpan duration, CacheInstanceType instanceTypeCode, bool extend = false) where T : class
      => redis.Set(key, value, duration, databaseSelector.Select(instanceTypeCode), extend);

    public bool Set<T>(string key, T value, DateTime duration, CacheInstanceType instanceTypeCode, bool extend = false) where T : class
      => redis.Set(key, value, duration, databaseSelector.Select(instanceTypeCode), extend);

    public async Task<bool> SetAsync<T>(string key, T value, int duration, CacheInstanceType instanceTypeCode, bool extend = false, CancellationToken token = default) where T : class
      => await redis.SetAsync(key, value, duration, databaseSelector.Select(instanceTypeCode), extend, token);

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan duration, CacheInstanceType instanceTypeCode, bool extend = false, CancellationToken token = default) where T : class
      => await redis.SetAsync(key, value, duration, databaseSelector.Select(instanceTypeCode), extend, token);

    public async Task<bool> SetAsync<T>(string key, T value, DateTime duration, CacheInstanceType instanceTypeCode, bool extend = false, CancellationToken token = default) where T : class
      => await redis.SetAsync(key, value, duration, databaseSelector.Select(instanceTypeCode), extend, token);
}