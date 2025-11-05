namespace Acontplus.Services.Services.Implementations;

/// <summary>
/// In-memory cache service implementation.
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly MemoryCacheOptions _options;

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger, IOptions<MemoryCacheOptions> options)
    {
        _cache = cache;
        _logger = logger;
        _options = options.Value;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return _cache.TryGetValue(key, out var value) ? Task.FromResult((T?)value) : Task.FromResult<T?>(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value for key: {Key}", key);
            return Task.FromResult<T?>(default);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration.Value;

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value for key: {Key}", key);
        }
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _cache.Remove(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key: {Key}", key);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = _cache.TryGetValue(key, out _);
            return Task.FromResult(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key: {Key}", key);
            return Task.FromResult(false);
        }
    }

    public CacheStatistics GetStatistics()
    {
        // Memory cache doesn't provide direct statistics, but we can estimate
        return new CacheStatistics
        {
            TotalEntries = _options.SizeLimit ?? 0,
            TotalMemoryBytes = GC.GetTotalMemory(false),
            HitRatePercentage = 0, // Memory cache doesn't track this
            MissRatePercentage = 0, // Memory cache doesn't track this
            Evictions = 0, // Memory cache doesn't track this
            LastCleanup = DateTime.UtcNow
        };
    }

    public T? Get<T>(string key)
    {
        try
        {
            return _cache.TryGetValue(key, out var value) ? (T?)value : default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value for key: {Key}", key);
            return default;
        }
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration.Value;

            _cache.Set(key, value, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value for key: {Key}", key);
        }
    }

    public void Remove(string key)
    {
        try
        {
            _cache.Remove(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key: {Key}", key);
        }
    }

    public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        try
        {
            if (_cache.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            var newValue = factory();
            Set(key, newValue, expiration);
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrCreate for key: {Key}", key);
            return factory();
        }
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_cache.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            var newValue = await factory();
            await SetAsync(key, newValue, expiration, cancellationToken);
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrCreateAsync for key: {Key}", key);
            return await factory();
        }
    }

    public void Clear()
    {
        try
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
        }
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        Clear();
        return Task.CompletedTask;
    }

    public bool Exists(string key)
    {
        try
        {
            return _cache.TryGetValue(key, out _);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key: {Key}", key);
            return false;
        }
    }
}
