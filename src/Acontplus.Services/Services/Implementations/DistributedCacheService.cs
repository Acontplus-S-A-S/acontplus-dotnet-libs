using Acontplus.Services.Services.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Acontplus.Services.Services.Implementations;

/// <summary>
/// Distributed cache service implementation using Redis or other distributed cache.
/// </summary>
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedCacheService> _logger;

    public DistributedCacheService(IDistributedCache cache, ILogger<DistributedCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        try
        {
            var value = _cache.GetString(key);
            if (string.IsNullOrEmpty(value))
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value for key: {Key}", key);
            return default;
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(value))
                return default;

            return JsonSerializer.Deserialize<T>(value);
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
            var jsonValue = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            _cache.SetString(key, jsonValue, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value for key: {Key}", key);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonValue = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            await _cache.SetStringAsync(key, jsonValue, options, cancellationToken);
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

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key: {Key}", key);
        }
    }

    public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        var value = Get<T>(key);
        if (value != null)
            return value;

        value = factory();
        Set(key, value, expiration);
        return value;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<T>(key, cancellationToken);
        if (value != null)
            return value;

        value = await factory();
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    public void Clear()
    {
        // Distributed cache doesn't support clearing all entries
        _logger.LogWarning("Clear operation not supported for distributed cache");
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // Distributed cache doesn't support clearing all entries
        _logger.LogWarning("Clear operation not supported for distributed cache");
        return Task.CompletedTask;
    }

    public bool Exists(string key)
    {
        try
        {
            var value = _cache.GetString(key);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _cache.GetStringAsync(key, cancellationToken);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key: {Key}", key);
            return false;
        }
    }

    public CacheStatistics GetStatistics()
    {
        // Distributed cache doesn't provide direct statistics
        return new CacheStatistics
        {
            TotalEntries = 0,
            TotalMemoryBytes = 0,
            HitRatePercentage = 0,
            MissRatePercentage = 0,
            Evictions = 0,
            LastCleanup = null
        };
    }
}
