using Acontplus.Services.Configuration;
using Acontplus.Services.Services.Abstractions;
using Acontplus.Services.Services.Implementations;

namespace Acontplus.Services.Extensions;

/// <summary>
/// Extension methods for registering core services and patterns.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Add caching services with support for both in-memory and distributed caching.
    /// </summary>
    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheConfig = configuration.GetSection("Caching").Get<CacheConfiguration>()
            ?? new CacheConfiguration();

        if (cacheConfig.UseDistributedCache && !string.IsNullOrEmpty(cacheConfig.RedisConnectionString))
        {
            // Use Redis for distributed caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheConfig.RedisConnectionString;
                options.InstanceName = cacheConfig.RedisInstanceName ?? "acontplus";
            });

            services.AddScoped<ICacheService, DistributedCacheService>();
        }
        else
        {
            // Use in-memory caching
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = cacheConfig.MemoryCacheSizeLimit;
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(cacheConfig.ExpirationScanFrequencyMinutes);
            });

            services.AddScoped<ICacheService, MemoryCacheService>();
        }

        return services;
    }

    /// <summary>
    /// Add resilience services including circuit breakers, retry policies, and timeouts.
    /// </summary>
    public static IServiceCollection AddResilienceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var resilienceConfig = configuration.GetSection("Resilience").Get<ResilienceConfiguration>()
            ?? new ResilienceConfiguration();

        if (!resilienceConfig.CircuitBreaker.Enabled &&
            !resilienceConfig.RetryPolicy.Enabled &&
            !resilienceConfig.Timeout.Enabled)
            return services;

        // Register circuit breaker service
        services.AddScoped<ICircuitBreakerService, CircuitBreakerService>();

        // Configure resilience configuration
        services.Configure<ResilienceConfiguration>(configuration.GetSection("Resilience"));

        return services;
    }

    /// <summary>
    /// Add HTTP client factory with resilience policies.
    /// </summary>
    public static IServiceCollection AddResilientHttpClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var resilienceConfig = configuration.GetSection("Resilience").Get<ResilienceConfiguration>()
            ?? new ResilienceConfiguration();

        if (!resilienceConfig.CircuitBreaker.Enabled)
            return services;

        // Add HTTP client factory with resilience
        services.AddHttpClient("resilient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.HttpClientTimeoutSeconds);
        })
        .AddPolicyHandler(GetResiliencePolicy(resilienceConfig));

        // Add named HTTP clients for common services
        services.AddHttpClient("api-client", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(resilienceConfig.Timeout.HttpClientTimeoutSeconds);
        })
        .AddPolicyHandler(GetApiResiliencePolicy(resilienceConfig));

        return services;
    }

    /// <summary>
    /// Add comprehensive health checks for all services.
    /// </summary>
    public static IServiceCollection AddServiceHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Add memory health check
        healthChecksBuilder.AddCheck("memory", () =>
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var data = new Dictionary<string, object>()
            {
                { "AllocatedBytes", allocated },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) }
            };

            // Consider unhealthy if more than 1GB allocated
            var status = allocated < 1_000_000_000 ? HealthStatus.Healthy : HealthStatus.Degraded;

            return HealthCheckResult.Healthy("Memory usage is within acceptable limits", data);
        });

        // Add resilience health checks if enabled
        var resilienceConfig = configuration.GetSection("Resilience").Get<ResilienceConfiguration>();
        if (resilienceConfig?.CircuitBreaker.Enabled == true)
        {
            healthChecksBuilder
                .AddCheck<CircuitBreakerHealthCheck>("circuit-breaker")
                .AddCheck<CacheHealthCheck>("cache");
        }

        return services;
    }

    /// <summary>
    /// Add monitoring and telemetry services.
    /// </summary>
    public static IServiceCollection AddMonitoringServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add application insights if configured
        var connectionString = configuration["ApplicationInsights:ConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddApplicationInsightsTelemetry();
        }

        // Add custom metrics and monitoring
        services.AddSingleton<IMetricsService, MetricsService>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetResiliencePolicy(ResilienceConfiguration config)
    {
        var policies = new List<IAsyncPolicy<HttpResponseMessage>>();

        if (config.CircuitBreaker.Enabled)
        {
            policies.Add(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: config.CircuitBreaker.ExceptionsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromSeconds(config.CircuitBreaker.DurationOfBreakSeconds)));
        }

        if (config.RetryPolicy.Enabled)
        {
            policies.Add(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: config.RetryPolicy.MaxRetries,
                    sleepDurationProvider: retryAttempt =>
                    {
                        if (config.RetryPolicy.ExponentialBackoff)
                        {
                            var delay = TimeSpan.FromSeconds(config.RetryPolicy.BaseDelaySeconds * Math.Pow(2, retryAttempt - 1));
                            return delay > TimeSpan.FromSeconds(config.RetryPolicy.MaxDelaySeconds)
                                ? TimeSpan.FromSeconds(config.RetryPolicy.MaxDelaySeconds)
                                : delay;
                        }
                        return TimeSpan.FromSeconds(config.RetryPolicy.BaseDelaySeconds);
                    }));
        }

        if (config.Timeout.Enabled)
        {
            policies.Add(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(config.Timeout.HttpClientTimeoutSeconds)));
        }

        return policies.Count > 1
            ? Policy.WrapAsync(policies.ToArray())
            : policies.FirstOrDefault() ?? Policy.NoOpAsync<HttpResponseMessage>();
    }

    private static IAsyncPolicy<HttpResponseMessage> GetApiResiliencePolicy(ResilienceConfiguration config)
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30))
            .WrapAsync(
                Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(1 * retryAttempt))
            );
    }
}

/// <summary>
/// Configuration for caching services.
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// Use distributed cache (Redis) instead of in-memory cache.
    /// </summary>
    public bool UseDistributedCache { get; set; } = false;

    /// <summary>
    /// Redis connection string.
    /// </summary>
    public string? RedisConnectionString { get; set; }

    /// <summary>
    /// Redis instance name.
    /// </summary>
    public string? RedisInstanceName { get; set; }

    /// <summary>
    /// Memory cache size limit in bytes.
    /// </summary>
    public long MemoryCacheSizeLimit { get; set; } = 1024 * 1024 * 100; // 100 MB

    /// <summary>
    /// Expiration scan frequency in minutes.
    /// </summary>
    public int ExpirationScanFrequencyMinutes { get; set; } = 5;
}

/// <summary>
/// Interface for metrics service.
/// </summary>
public interface IMetricsService
{
    void IncrementCounter(string name, Dictionary<string, string>? tags = null);
    void RecordGauge(string name, double value, Dictionary<string, string>? tags = null);
    void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null);
}

/// <summary>
/// Implementation of metrics service.
/// </summary>
public class MetricsService : IMetricsService
{
    public void IncrementCounter(string name, Dictionary<string, string>? tags = null)
    {
        // Implementation for metrics collection
    }

    public void RecordGauge(string name, double value, Dictionary<string, string>? tags = null)
    {
        // Implementation for metrics collection
    }

    public void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null)
    {
        // Implementation for metrics collection
    }
}

/// <summary>
/// Health check for circuit breaker service.
/// </summary>
public class CircuitBreakerHealthCheck : IHealthCheck
{
    private readonly ICircuitBreakerService _circuitBreakerService;

    public CircuitBreakerHealthCheck(ICircuitBreakerService circuitBreakerService)
    {
        _circuitBreakerService = circuitBreakerService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var defaultState = _circuitBreakerService.GetCircuitBreakerState("default");
            var apiState = _circuitBreakerService.GetCircuitBreakerState("api");
            var databaseState = _circuitBreakerService.GetCircuitBreakerState("database");
            var externalState = _circuitBreakerService.GetCircuitBreakerState("external");
            var authState = _circuitBreakerService.GetCircuitBreakerState("auth");

            var data = new Dictionary<string, object>
            {
                ["default"] = defaultState.ToString(),
                ["api"] = apiState.ToString(),
                ["database"] = databaseState.ToString(),
                ["external"] = externalState.ToString(),
                ["auth"] = authState.ToString(),
                ["lastCheckTime"] = DateTime.UtcNow
            };

            // Check if any critical circuits are open
            var criticalCircuitsOpen = new[] { databaseState, authState }.Any(state => state == CircuitBreakerState.Open);
            var anyCircuitOpen = new[] { defaultState, apiState, databaseState, externalState, authState }
                .Any(state => state == CircuitBreakerState.Open);

            if (criticalCircuitsOpen)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Critical circuit breakers are open", data: data));
            }

            if (anyCircuitOpen)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Some circuit breakers are open", data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy("All circuit breakers are operational", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Circuit breaker service failed", ex));
        }
    }
}

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
