namespace Edition.Domain.Caching.Abstractions;

/// <summary>
/// Represents a distributed cache of serialized values.
/// </summary>
public interface IRedisCache
{
    /// <summary>
    /// Gets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <returns>The located value or null.</returns>
    T? Get<T>(string key, int dbIndex = 0);

    /// <summary>
    /// Gets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <returns>The located value or null.</returns>
    string? Get(string key, int dbIndex = 0);

    /// <summary>
    /// Gets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located value or null.</returns>
    Task<T?> GetAsync<T>(string key, int dbIndex = 0, CancellationToken token = default);

    /// <summary>
    /// Sets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="duration">duration to store based on seconds</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="extend">if key is exists and extend is true, key will be replace</param>
    bool Set<T>(string key, T value, int duration, int dbIndex = 0, bool extend = false) where T : class;

    /// <summary>
    /// Sets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="duration">duration to store based on TimeSpan</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="extend">if key is exists and extend is true, key will be replace</param>
    bool Set<T>(string key, T value, TimeSpan duration, int dbIndex = 0, bool extend = false) where T : class;

    /// <summary>
    /// Sets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="duration">duration to store based on DateTime</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="extend">if key is exists and extend is true, key will be replace</param>
    bool Set<T>(string key, T value, DateTime duration, int dbIndex = 0, bool extend = false) where T : class;

    /// <summary>
    /// Sets the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="duration">duration to store based on seconds</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="extend">if key is exists and extend is true, key will be replace</param>
    /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task<bool> SetAsync<T>(string key, T value, int duration, int dbIndex = 0, bool extend = false, CancellationToken token = default) where T : class;

    /// <summary>
    /// Sets the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="duration">duration to store based on TimeSpan</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="extend">if key is exists and extend is true, key will be replace</param>
    /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task<bool> SetAsync<T>(string key, T value, TimeSpan duration, int dbIndex = 0, bool extend = false, CancellationToken token = default) where T : class;

    /// <summary>
    /// Sets the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="duration">duration to store based on DateTime</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="extend">if key is exists and extend is true, key will be replace</param>
    /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task<bool> SetAsync<T>(string key, T value, DateTime duration, int dbIndex = 0, bool extend = false, CancellationToken token = default) where T : class;

    /// <summary>
    /// Refreshes a value in the cache based on its key, resetting its sliding expiration timeout (if any).
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    void Refresh(string key);

    /// <summary>
    /// Refreshes a value in the cache based on its key, resetting its sliding expiration timeout (if any).
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task RefreshAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Removes the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="dbIndex">Redis db index</param>
    bool Remove(string key, int dbIndex = 0);

    /// <summary>
    /// Removes the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="dbIndex">Redis db index</param>
    /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task<bool> RemoveAsync(string key, int dbIndex = 0, CancellationToken token = default);
}