namespace Acontplus.Services.Services.Abstractions;

/// <summary>
/// Interface for caching service supporting both in-memory and distributed caching.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get a value from cache.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value or default if not found.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Get a value from cache asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value or default if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set a value in cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Optional expiration time.</param>
    void Set<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Set a value in cache asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a value from cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    void Remove(string key);

    /// <summary>
    /// Remove a value from cache asynchronously.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get or create a value from cache.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">Factory function to create the value if not cached.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <returns>The cached or newly created value.</returns>
    T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null);

    /// <summary>
    /// Get or create a value from cache asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">Factory function to create the value if not cached.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly created value.</returns>
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear all cache entries.
    /// </summary>
    void Clear();

    /// <summary>
    /// Clear all cache entries asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cache statistics.
    /// </summary>
    /// <returns>Cache statistics information.</returns>
    CacheStatistics GetStatistics();

    /// <summary>
    /// Check if a key exists in cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    bool Exists(string key);

    /// <summary>
    /// Check if a key exists in cache asynchronously.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Cache statistics information.
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Total number of cache entries.
    /// </summary>
    public long TotalEntries { get; set; }

    /// <summary>
    /// Total memory usage in bytes.
    /// </summary>
    public long TotalMemoryBytes { get; set; }

    /// <summary>
    /// Cache hit rate percentage.
    /// </summary>
    public double HitRatePercentage { get; set; }

    /// <summary>
    /// Cache miss rate percentage.
    /// </summary>
    public double MissRatePercentage { get; set; }

    /// <summary>
    /// Number of evictions.
    /// </summary>
    public long Evictions { get; set; }

    /// <summary>
    /// Last cleanup time.
    /// </summary>
    public DateTime? LastCleanup { get; set; }
}
