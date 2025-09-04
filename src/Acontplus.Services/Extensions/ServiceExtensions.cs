using Acontplus.Services.Configuration;
using Acontplus.Services.HealthChecks;
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


