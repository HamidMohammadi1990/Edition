using Edition.Domain.Caching.Enums;

namespace Edition.Domain.Caching.Abstractions;

public interface IDatabaseSelector
{
    /// <summary>
    /// return db index based on <paramref name="cacheInstance"/>
    /// </summary>
    /// <param name="cacheInstance">CacheInstanceType</param>
    /// <returns></returns>
    int Select(CacheInstanceType cacheInstance);
}