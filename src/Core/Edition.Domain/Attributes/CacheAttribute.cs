using Edition.Domain.Caching.Enums;

namespace Edition.Domain.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CacheAttribute : Attribute
{
    public int Duration { get; private init; }
    public CacheInstanceType CacheInstance { get; private init; }
    public bool Extend { get; private init; }
    /// <summary>
    /// Indicate that this method automatically cache
    /// </summary>
    /// <param name="duration">Duration is based on minute</param>
    /// <param name="cacheInstance">Choose cache instance</param>
    /// <param name="extend">Indicate if cache exists, replace it with new value or not. If extend is true, it will replace with new value</param>
    public CacheAttribute(int duration, CacheInstanceType cacheInstance, bool extend = false)
    {
        Extend = extend;
        Duration = duration * 60;
        CacheInstance = cacheInstance;
    }
    public CacheAttribute(TimeSpan duration, CacheInstanceType cacheInstance, bool extend = false)
    {
        switch (duration.TotalSeconds)
        {
            case <= 0:
                throw new Exception($"{nameof(duration)} must be greater than 0!");
            case > int.MaxValue:
                throw new Exception($"{nameof(duration)} must be less than {int.MaxValue} seconds!");
        }

        Duration = (int)duration.TotalSeconds;
        CacheInstance = cacheInstance;
        Extend = extend;
    }
}