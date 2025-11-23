namespace Acontplus.Core.Abstractions.Infrastructure.Caching;

/// <summary>
/// Interface for caching service supporting both in-memory and distributed caching.
/// </summary>
public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    void Remove(string key);
    bool TryGetValue<T>(string key, out T? value);
    T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null);

    // Async versions
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken ct = default);
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
