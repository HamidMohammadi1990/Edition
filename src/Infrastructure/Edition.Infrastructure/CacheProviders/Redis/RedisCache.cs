using StackExchange.Redis;
using Edition.Common.Extensions;
using Edition.Domain.Caching.Abstractions;

namespace Edition.Infrastructure.CacheProviders.Redis;

public class RedisCache : IRedisCache
{
    private readonly IConnectionMultiplexer connection;

    public RedisCache(IConnectionMultiplexer connection) => this.connection = connection;

    public T? Get<T>(string key, int dbIndex = 0) => GetAsync<T>(key, dbIndex).Result;

    private static string? GetKey(string? key) => string.IsNullOrEmpty(key) ? null : key.Replace(" ", "").Trim();

    public string? Get(string key, int dbIndex = 0) => GetAsync<string>(key, dbIndex).Result;

    public async Task<T?> GetAsync<T>(string? key, int dbIndex = 0, CancellationToken token = default)
    {
        key = GetKey(key);
        if (key is null) return default;

        token.ThrowIfCancellationRequested();

        var db = connection.GetDatabase(dbIndex);

        if (typeof(T).IsPrimitive)
        {
            var cacheValue = await db.StringGetAsync(key);
            return GetValue<T>(cacheValue);
        }
        var cacheBinary = await db.StringGetAsync(key);
        var result = cacheBinary.Deserialize<T>();
        return result ?? default;
    }

    public void Refresh(string key)
    {
        throw new NotImplementedException();
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public bool Remove(string? key, int dbIndex = 0)
    {
        key = GetKey(key);
        if (key is null) return false;

        var db = connection.GetDatabase(dbIndex);
        return db.KeyDelete(key);
    }

    public async Task<bool> RemoveAsync(string? key, int dbIndex = 0, CancellationToken token = default)
    {
        key = GetKey(key);
        if (key is null) return false;
        var db = connection.GetDatabase(dbIndex);
        return await db.KeyDeleteAsync(key);
    }


    public bool Set<T>(string key, T value, TimeSpan duration, int dbIndex = 0, bool extend = false) where T : class
    {
        return SetAsync(key, value, duration, dbIndex, extend).Result;
    }
    public bool Set<T>(string key, T value, int duration, int dbIndex = 0, bool extend = false) where T : class
    {
        var timeSpan = DateTime.Now.AddSeconds(duration) - DateTime.Now;
        return Set(key, value, timeSpan, dbIndex, extend);
    }

    public bool Set<T>(string key, T value, DateTime duration, int dbIndex = 0, bool extend = false) where T : class
    {
        var timeSpan = duration - DateTime.Now;
        return Set(key, value, timeSpan, dbIndex, extend);
    }

    public async Task<bool> SetAsync<T>(string? key, T value, TimeSpan duration, int dbIndex = 0, bool extend = false, CancellationToken token = default) where T : class
    {
        key = GetKey(key);
        if (key is null) return false;

        token.ThrowIfCancellationRequested();

        var db = connection.GetDatabase(dbIndex);
        if (!extend && await db.KeyExistsAsync(key)) return false;
        if (typeof(T).IsPrimitive)
        {
            await db.StringSetAsync(key, value.ToString(), duration);
            return true;
        }
        await db.StringSetAsync(key, value.Serialize(), duration);
        return true;
    }

    public async Task<bool> SetAsync<T>(string key, T value, DateTime duration, int dbIndex = 0, bool extend = false, CancellationToken token = default) where T : class
    {
        var timeSpan = duration - DateTime.Now;
        return await SetAsync(key, value, timeSpan, dbIndex, extend, token);
    }

    public async Task<bool> SetAsync<T>(string key, T value, int duration, int dbIndex = 0, bool extend = false, CancellationToken token = default) where T : class
    {
        var timeSpan = DateTime.Now.AddSeconds(duration) - DateTime.Now;      
        return await SetAsync(key, value, timeSpan, dbIndex, extend, token);
    }

    private static T GetValue<T>(string value) => (T)Convert.ChangeType(value, typeof(T));
}