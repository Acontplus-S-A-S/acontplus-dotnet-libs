using System.Collections.Concurrent;

namespace Acontplus.Reports.Services;

/// <summary>
/// Cache entry for report definitions with expiration
/// </summary>
internal class CachedReportDefinition : IDisposable
{
    public MemoryStream Stream { get; }
    public DateTime CreatedAt { get; }
    public DateTime LastAccessedAt { get; set; }
    private bool _disposed;

    public CachedReportDefinition(MemoryStream stream)
    {
        Stream = stream;
        CreatedAt = DateTime.UtcNow;
        LastAccessedAt = DateTime.UtcNow;
    }

    public bool IsExpired(TimeSpan ttl)
    {
        return DateTime.UtcNow - CreatedAt > ttl;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Stream?.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// Thread-safe cache for report definitions with size limits and TTL
/// </summary>
public class ReportDefinitionCache : IDisposable
{
    private readonly ConcurrentDictionary<string, CachedReportDefinition> _cache = new();
    private readonly int _maxSize;
    private readonly TimeSpan _ttl;
    private readonly SemaphoreSlim _cleanupLock = new(1, 1);
    private bool _disposed;

    public ReportDefinitionCache(int maxSize, TimeSpan ttl)
    {
        _maxSize = maxSize;
        _ttl = ttl;
    }

    public async Task<MemoryStream> GetOrAddAsync(string key, Func<string, Task<MemoryStream>> factory)
    {
        // Try to get existing non-expired entry
        if (_cache.TryGetValue(key, out var cached))
        {
            if (!cached.IsExpired(_ttl))
            {
                cached.LastAccessedAt = DateTime.UtcNow;
                // Create a copy to avoid thread safety issues
                var copy = new MemoryStream();
                cached.Stream.Position = 0;
                await cached.Stream.CopyToAsync(copy);
                copy.Position = 0;
                return copy;
            }
            else
            {
                // Remove expired entry
                _cache.TryRemove(key, out var removed);
                removed?.Dispose();
            }
        }

        // Cleanup if cache is too large
        if (_cache.Count >= _maxSize)
        {
            await CleanupOldEntriesAsync();
        }

        // Create new entry
        var stream = await factory(key);
        var cachedEntry = new CachedReportDefinition(stream);
        _cache.TryAdd(key, cachedEntry);

        // Return a copy
        var result = new MemoryStream();
        stream.Position = 0;
        await stream.CopyToAsync(result);
        result.Position = 0;
        return result;
    }

    private async Task CleanupOldEntriesAsync()
    {
        await _cleanupLock.WaitAsync();
        try
        {
            // Remove expired entries first
            var expiredKeys = _cache
                .Where(kvp => kvp.Value.IsExpired(_ttl))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                if (_cache.TryRemove(key, out var removed))
                {
                    removed.Dispose();
                }
            }

            // If still too many, remove least recently used
            if (_cache.Count >= _maxSize)
            {
                var toRemove = _cache
                    .OrderBy(kvp => kvp.Value.LastAccessedAt)
                    .Take(_cache.Count - _maxSize + 10) // Remove extra to avoid frequent cleanups
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in toRemove)
                {
                    if (_cache.TryRemove(key, out var removed))
                    {
                        removed.Dispose();
                    }
                }
            }
        }
        finally
        {
            _cleanupLock.Release();
        }
    }

    public void Clear()
    {
        foreach (var entry in _cache.Values)
        {
            entry.Dispose();
        }
        _cache.Clear();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Clear();
            _cleanupLock.Dispose();
            _disposed = true;
        }
    }
}
