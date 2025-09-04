using Acontplus.Services.Services.Abstractions;

namespace Acontplus.Services.HealthChecks;

/// <summary>
/// Health check for cache service.
/// </summary>
public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;

    public CacheHealthCheck(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Test cache functionality with a comprehensive test
            var testKey = $"health-check-{Guid.NewGuid():N}";
            var testValue = $"test-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}";
            var testExpiration = TimeSpan.FromSeconds(30);

            // Test Set operation
            await _cacheService.SetAsync(testKey, testValue, testExpiration, cancellationToken);

            // Test Get operation
            var retrieved = await _cacheService.GetAsync<string>(testKey, cancellationToken);

            // Test Exists operation
            var exists = await _cacheService.ExistsAsync(testKey, cancellationToken);

            // Test Remove operation
            await _cacheService.RemoveAsync(testKey, cancellationToken);

            // Verify removal
            var removedValue = await _cacheService.GetAsync<string>(testKey, cancellationToken);

            if (retrieved == testValue && exists && removedValue == null)
            {
                var stats = _cacheService.GetStatistics();
                var data = new Dictionary<string, object>
                {
                    ["totalEntries"] = stats.TotalEntries,
                    ["hitRatePercentage"] = stats.HitRatePercentage,
                    ["lastTestTime"] = DateTime.UtcNow
                };
                return HealthCheckResult.Healthy("Cache service is fully operational", data);
            }

            return HealthCheckResult.Degraded("Cache service test partially failed - some operations may not be working correctly");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cache service failed", ex);
        }
    }
}
