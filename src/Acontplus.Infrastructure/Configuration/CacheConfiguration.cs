namespace Acontplus.Infrastructure.Configuration;

/// <summary>
///     Configuration for caching services.
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    ///     Use distributed cache (Redis) instead of in-memory cache.
    /// </summary>
    public bool UseDistributedCache { get; set; } = false;

    /// <summary>
    ///     Redis connection string.
    /// </summary>
    public string? RedisConnectionString { get; set; }

    /// <summary>
    ///     Redis instance name.
    /// </summary>
    public string? RedisInstanceName { get; set; }

    /// <summary>
    ///     Memory cache size limit in bytes.
    /// </summary>
    public long MemoryCacheSizeLimit { get; set; } = 1024 * 1024 * 100; // 100 MB

    /// <summary>
    ///     Expiration scan frequency in minutes.
    /// </summary>
    public int ExpirationScanFrequencyMinutes { get; set; } = 5;
}
