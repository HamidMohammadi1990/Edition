using System.Diagnostics;
using Edition.Domain.Caching.Enums;
using Edition.Domain.Caching.Abstractions;

namespace Edition.Infrastructure.CacheProviders;

public class DatabaseSelector : IDatabaseSelector
{
    [DebuggerStepThrough]
    public int Select(CacheInstanceType cacheInstance) => cacheInstance switch
    {
        CacheInstanceType.Default or CacheInstanceType.AppSettings or CacheInstanceType.Default => 0,
        _ => 5,
    };
}