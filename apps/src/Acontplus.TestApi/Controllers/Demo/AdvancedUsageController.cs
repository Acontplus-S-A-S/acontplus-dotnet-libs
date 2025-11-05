using Acontplus.Core.Domain.Enums;
using Acontplus.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace Acontplus.TestApi.Controllers.Demo;

/// <summary>
/// Controller demonstrating advanced usage patterns from Acontplus.Services README.
/// Includes basic, intermediate, and enterprise usage examples.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AdvancedUsageController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly ICircuitBreakerService _circuitBreaker;
    private readonly IRequestContextService _requestContext;
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly ISecurityHeaderService _securityHeaders;
    private readonly ILogger<AdvancedUsageController> _logger;

    public AdvancedUsageController(
        ICacheService cache,
        ICircuitBreakerService circuitBreaker,
        IRequestContextService requestContext,
        IDeviceDetectionService deviceDetection,
        ISecurityHeaderService securityHeaders,
        ILogger<AdvancedUsageController> logger)
    {
        _cache = cache;
        _circuitBreaker = circuitBreaker;
        _requestContext = requestContext;
        _deviceDetection = deviceDetection;
        _securityHeaders = securityHeaders;
        _logger = logger;
    }

    #region Basic Usage Examples

    /// <summary>
    /// Basic usage - Simple caching and request context.
    /// </summary>
    [HttpGet("basic/hello")]
    public async Task<IActionResult> BasicHello()
    {
        var message = await _cache.GetOrCreateAsync(
            "hello-message",
            () => Task.FromResult("Hello from Acontplus.Services!"),
            TimeSpan.FromMinutes(5)
        );

        return Ok(new
        {
            Message = message,
            CorrelationId = _requestContext.GetCorrelationId()
        });
    }

    /// <summary>
    /// Basic usage - Simple product retrieval with caching.
    /// </summary>
    [HttpGet("basic/products/{id:int}")]
    public async Task<IActionResult> GetBasicProduct(int id)
    {
        var cacheKey = $"product:{id}";

        var product = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            // Simulate database call
            await Task.Delay(100);
            return new BasicProduct { Id = id, Name = $"Product {id}", Price = id * 10.99m };
        }, TimeSpan.FromMinutes(15));

        return Ok(new
        {
            Product = product,
            CorrelationId = _requestContext.GetCorrelationId(),
            Cached = true
        });
    }

    #endregion

    #region Intermediate Usage Examples

    /// <summary>
    /// Intermediate usage - Device-aware content with caching and circuit breaker.
    /// </summary>
    [HttpGet("intermediate/content")]
    public async Task<IActionResult> GetIntermediateContent()
    {
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        var cacheKey = $"content:{deviceType}";

        var content = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            // Simulate external API call with circuit breaker
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                await Task.Delay(100); // Simulate API call
                return deviceType switch
                {
                    DeviceType.Mobile => "Mobile-optimized content with touch-friendly UI",
                    DeviceType.Tablet => "Tablet-optimized content with larger touch targets",
                    _ => "Desktop content with hover effects and keyboard shortcuts"
                };
            }, "content-api");
        }, TimeSpan.FromMinutes(10));

        return Ok(new
        {
            Content = content,
            DeviceType = deviceType.ToString(),
            CacheKey = cacheKey
        });
    }

    /// <summary>
    /// Intermediate usage - Health monitoring for services.
    /// </summary>
    [HttpGet("intermediate/health")]
    public IActionResult GetIntermediateHealth()
    {
        var circuitBreakerStatus = _circuitBreaker.GetCircuitBreakerState("content-api");
        var cacheStats = _cache.GetStatistics();

        return Ok(new
        {
            CircuitBreaker = new
            {
                Name = "content-api",
                State = circuitBreakerStatus.ToString(),
                IsHealthy = circuitBreakerStatus == CircuitBreakerState.Closed
            },
            Cache = new
            {
                TotalEntries = cacheStats.TotalEntries,
                HitRate = $"{cacheStats.HitRatePercentage:F1}%",
                MemoryUsage = $"{cacheStats.TotalMemoryBytes / 1024 / 1024:F1} MB"
            },
            Services = new
            {
                Cache = "Healthy",
                CircuitBreaker = circuitBreakerStatus == CircuitBreakerState.Closed ? "Healthy" : "Degraded",
                DeviceDetection = "Healthy"
            }
        });
    }

    /// <summary>
    /// Intermediate usage - Cache performance monitoring.
    /// </summary>
    [HttpGet("intermediate/cache-stats")]
    public IActionResult GetCacheStats()
    {
        var stats = _cache.GetStatistics();

        // Calculate additional metrics
        var efficiency = stats.TotalEntries > 0
            ? (stats.HitRatePercentage / 100.0) * stats.TotalEntries
            : 0;

        return Ok(new
        {
            TotalEntries = stats.TotalEntries,
            HitRatePercentage = stats.HitRatePercentage,
            MissRatePercentage = stats.MissRatePercentage,
            MemoryUsageBytes = stats.TotalMemoryBytes,
            MemoryUsageMB = stats.TotalMemoryBytes / 1024 / 1024,
            Efficiency = efficiency,
            LastCleanup = stats.LastCleanup,
            Evictions = stats.Evictions
        });
    }

    #endregion

    #region Advanced Usage Examples

    /// <summary>
    /// Advanced usage - Multi-tenant dashboard with comprehensive monitoring.
    /// </summary>
    [HttpGet("advanced/dashboard")]
    [Authorize(Policy = "RequireClientId")]
    public async Task<IActionResult> GetAdvancedDashboard()
    {
        var tenantId = _requestContext.GetTenantId();
        var clientId = _requestContext.GetClientId();
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);

        var cacheKey = $"dashboard:{tenantId}:{clientId}:{deviceType}";

        var dashboard = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                // Simulate complex dashboard data retrieval
                await Task.Delay(200);
                return new AdvancedDashboard
                {
                    TenantId = tenantId,
                    ClientId = clientId,
                    DeviceType = deviceType.ToString(),
                    Data = new DashboardData
                    {
                        Metrics = new DashboardMetrics
                        {
                            Requests = 1500,
                            Errors = 2,
                            Uptime = 99.8,
                            ResponseTime = 245.5
                        },
                        RecentActivity = new[]
                        {
                            "User login at 10:30 AM",
                            "Data export completed",
                            "Report generated successfully",
                            "Cache hit rate: 87.3%"
                        },
                        Alerts = new[]
                        {
                            "High memory usage detected",
                            "Database slow queries warning",
                            "Circuit breaker 'payment-api' is open"
                        }
                    },
                    GeneratedAt = DateTime.UtcNow
                };
            }, "dashboard-service");
        }, TimeSpan.FromMinutes(5));

        return Ok(dashboard);
    }

    /// <summary>
    /// Advanced usage - Audit logging with comprehensive context.
    /// </summary>
    [HttpPost("advanced/audit")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult LogAuditEvent([FromBody] AuditEvent auditEvent)
    {
        var auditLog = new AdvancedAuditLog
        {
            Event = auditEvent,
            Context = new AuditContext
            {
                CorrelationId = _requestContext.GetCorrelationId(),
                TenantId = _requestContext.GetTenantId(),
                ClientId = _requestContext.GetClientId(),
                UserId = User.Identity?.Name,
                Timestamp = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                RequestPath = Request.Path.ToString(),
                RequestMethod = Request.Method
            }
        };

        // In a real application, you would log this to your audit system
        _logger.LogInformation("Enterprise audit event: {@AuditLog}", auditLog);

        return Ok(new
        {
            Message = "Audit event logged successfully",
            CorrelationId = _requestContext.GetCorrelationId(),
            AuditId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Advanced usage - Security status and monitoring.
    /// </summary>
    [HttpGet("advanced/security-status")]
    public IActionResult GetAdvancedSecurityStatus()
    {
        var headers = _securityHeaders.GetRecommendedHeaders(false);
        var cspNonce = _securityHeaders.GenerateCspNonce();

        return Ok(new AdvancedSecurityStatus
        {
            SecurityHeaders = headers,
            CspNonce = cspNonce,
            CircuitBreakerStatus = new Dictionary<string, object>
            {
                ["dashboard-service"] = _circuitBreaker.GetCircuitBreakerState("dashboard-service"),
                ["auth-service"] = _circuitBreaker.GetCircuitBreakerState("auth-service"),
                ["database-service"] = _circuitBreaker.GetCircuitBreakerState("database-service"),
                ["payment-service"] = _circuitBreaker.GetCircuitBreakerState("payment-service")
            },
            SecurityMetrics = new SecurityMetrics
            {
                LastSecurityScan = DateTime.UtcNow.AddHours(-2),
                VulnerabilitiesFound = 0,
                SecurityScore = 95,
                ComplianceStatus = "Compliant"
            }
        });
    }

    /// <summary>
    /// Advanced usage - Multi-tenant data with isolation.
    /// </summary>
    [HttpGet("advanced/tenant-data/{dataType}")]
    [Authorize(Policy = "RequireTenant")]
    public async Task<IActionResult> GetTenantData(string dataType)
    {
        var tenantId = _requestContext.GetTenantId();
        var clientId = _requestContext.GetClientId();

        if (string.IsNullOrEmpty(tenantId))
        {
            return Unauthorized(new { Error = "Tenant ID is required" });
        }

        var cacheKey = $"tenant:{tenantId}:{clientId}:{dataType}";

        var tenantData = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Retrieving {DataType} for tenant {TenantId} and client {ClientId}",
                dataType, tenantId, clientId);

            // Simulate tenant-specific data retrieval
            await Task.Delay(100);

            return new TenantData
            {
                TenantId = tenantId,
                ClientId = clientId,
                DataType = dataType,
                Content = $"Advanced tenant-specific {dataType} for {tenantId}",
                LastUpdated = DateTime.UtcNow,
                Metadata = new Dictionary<string, object>
                {
                    ["tenant-tier"] = GetTenantTier(tenantId) ?? "default",
                    ["data-version"] = "2.0",
                    ["cache-key"] = cacheKey,
                    ["access-level"] = "advanced",
                    ["encryption"] = "AES-256",
                    ["backup-frequency"] = "hourly"
                }
            };
        }, TimeSpan.FromMinutes(15));

        return Ok(tenantData);
    }

    #endregion

    #region Advanced Service Patterns

    /// <summary>
    /// Advanced pattern - Hybrid caching with fallback.
    /// </summary>
    [HttpGet("advanced/hybrid-cache/{key}")]
    public async Task<IActionResult> GetWithHybridCache(string key)
    {
        try
        {
            // Try to get from cache first
            var value = await _cache.GetAsync<string>(key);
            if (value != null)
            {
                return Ok(new { Value = value, Source = "cache", CacheKey = key });
            }

            // Generate new value and cache it
            value = await GenerateValueAsync(key);
            await _cache.SetAsync(key, value, TimeSpan.FromMinutes(10));

            return Ok(new { Value = value, Source = "generated", CacheKey = key });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache operation failed for key: {Key}", key);

            // Fallback to generated value without caching
            var fallbackValue = await GenerateValueAsync(key);
            return Ok(new { Value = fallbackValue, Source = "fallback", CacheKey = key });
        }
    }

    /// <summary>
    /// Advanced pattern - Composite resilience with fallback.
    /// </summary>
    [HttpGet("advanced/resilient/{operation}")]
    public async Task<IActionResult> ExecuteResilientOperation(string operation)
    {
        var options = new ResilienceOptions
        {
            RetryCount = 2,
            RetryDelay = 1000,
            FallbackValue = $"Fallback result for {operation}"
        };

        try
        {
            var result = await ExecuteWithResilienceAsync(
                () => SimulateOperationAsync(operation),
                operation,
                options);

            return Ok(new { Result = result, Operation = operation, Strategy = "resilient" });
        }
        catch (Exception)
        {
            return StatusCode(503, new
            {
                Error = "Operation failed after all retry attempts",
                Operation = operation,
                FallbackUsed = true,
                FallbackValue = options.FallbackValue
            });
        }
    }

    /// <summary>
    /// Advanced pattern - Device-aware content optimization.
    /// </summary>
    [HttpGet("advanced/adaptive-content/{contentId}")]
    public async Task<IActionResult> GetAdaptiveContent(string contentId)
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var capabilities = _deviceDetection.GetDeviceCapabilities(userAgent);
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);

        var cacheKey = $"content:{contentId}:{deviceType}:{capabilities.Browser}";

        var adaptiveContent = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var baseContent = await GetBaseContentAsync(contentId);

            return new AdaptiveContent
            {
                Id = contentId,
                Title = baseContent.Title,
                Content = OptimizeContentForDevice(baseContent.Content, deviceType, capabilities),
                Media = GetOptimizedMedia(baseContent.Media, deviceType, capabilities),
                Layout = GetLayoutForDevice(deviceType, capabilities),
                Features = GetSupportedFeatures(capabilities),
                OptimizedFor = deviceType.ToString(),
                Browser = capabilities.Browser ?? "Unknown",
                TouchSupport = capabilities.SupportsTouch
            };
        }, TimeSpan.FromMinutes(15));

        return Ok(adaptiveContent);
    }

    #endregion

    #region Utility Methods

    private string GetTenantTier(string tenantId)
    {
        return tenantId.StartsWith("premium") ? "premium" : "standard";
    }

    private async Task<string> GenerateValueAsync(string key)
    {
        await Task.Delay(50);
        return $"Generated value for {key} at {DateTime.UtcNow:HH:mm:ss}";
    }

    private async Task<string> SimulateOperationAsync(string operation)
    {
        await Task.Delay(100);

        // Simulate occasional failures
        return Random.Shared.Next(1, 10) == 1
            ? throw new InvalidOperationException($"Simulated failure for {operation}")
            : $"Successfully completed {operation}";
    }

    private async Task<T> ExecuteWithResilienceAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        ResilienceOptions options)
    {
        try
        {
            return await _circuitBreaker.ExecuteAsync(operation, operationName);
        }
        catch (Exception ex) when (options.FallbackValue != null)
        {
            _logger.LogWarning(ex, "Operation {OperationName} failed, using fallback", operationName);
            return (T)options.FallbackValue;
        }
        catch (Exception ex) when (options.RetryCount > 0)
        {
            _logger.LogInformation(ex, "Retrying operation {OperationName}, attempt 1", operationName);

            for (int attempt = 1; attempt <= options.RetryCount; attempt++)
            {
                try
                {
                    await Task.Delay(options.RetryDelay * attempt);
                    return await _circuitBreaker.ExecuteAsync(operation, operationName);
                }
                catch (Exception retryEx) when (attempt < options.RetryCount)
                {
                    _logger.LogInformation(retryEx,
                        "Retry attempt {Attempt} failed for {OperationName}",
                        attempt + 1, operationName);
                }
            }

            throw new ResilienceException($"Operation {operationName} failed after {options.RetryCount} retries", ex);
        }
    }

    private async Task<BaseContent> GetBaseContentAsync(string contentId)
    {
        await Task.Delay(50);
        return new BaseContent
        {
            Id = contentId,
            Title = $"Content {contentId}",
            Content = "This is the base content that will be optimized for different devices.",
            Media = new[] { "image1.jpg", "video1.mp4" }
        };
    }

    private string OptimizeContentForDevice(string content, DeviceType deviceType, DeviceCapabilities capabilities)
    {
        return deviceType switch
        {
            DeviceType.Mobile when !capabilities.SupportsTouch =>
                content.Replace("touch-friendly", "mobile-optimized"),
            DeviceType.Mobile when capabilities.SupportsTouch =>
                content.Replace("mobile-optimized", "touch-friendly"),
            DeviceType.Tablet =>
                content.Replace("desktop", "tablet-optimized"),
            _ => content
        };
    }

    private object GetOptimizedMedia(string[] media, DeviceType deviceType, DeviceCapabilities capabilities)
    {
        return deviceType switch
        {
            DeviceType.Mobile => new
            {
                Images = media.Where(m => m.EndsWith(".jpg")).Select(m => $"mobile-{m}"),
                Videos = media.Where(m => m.EndsWith(".mp4")).Select(m => $"mobile-{m}"),
                Compression = "high",
                Format = "webp"
            },
            DeviceType.Tablet => new
            {
                Images = media.Where(m => m.EndsWith(".jpg")).Select(m => $"tablet-{m}"),
                Videos = media.Where(m => m.EndsWith(".mp4")).Select(m => $"tablet-{m}"),
                Compression = "medium",
                Format = "jpeg"
            },
            _ => new
            {
                Images = media.Where(m => m.EndsWith(".jpg")),
                Videos = media.Where(m => m.EndsWith(".mp4")),
                Compression = "low",
                Format = "original"
            }
        };
    }

    private object GetLayoutForDevice(DeviceType deviceType, DeviceCapabilities capabilities)
    {
        return deviceType switch
        {
            DeviceType.Mobile => new
            {
                Columns = 1,
                Spacing = "compact",
                TouchTargets = "large",
                SwipeGestures = capabilities.SupportsTouch,
                Navigation = "bottom-tabs"
            },
            DeviceType.Tablet => new
            {
                Columns = 2,
                Spacing = "comfortable",
                TouchTargets = "medium",
                SwipeGestures = capabilities.SupportsTouch,
                Navigation = "side-menu"
            },
            _ => new
            {
                Columns = 3,
                Spacing = "spacious",
                TouchTargets = "standard",
                HoverEffects = true,
                Navigation = "top-menu"
            }
        };
    }

    private string[] GetSupportedFeatures(DeviceCapabilities capabilities)
    {
        var features = new List<string> { "Responsive design" };

        if (capabilities.SupportsTouch)
            features.Add("Touch gestures");

        if (capabilities.Browser != null && capabilities.Browser.Contains("Chrome"))
            features.Add("PWA support");

        if (capabilities.OperatingSystem != null && capabilities.OperatingSystem.Contains("iOS"))
            features.Add("Apple Pay");

        return features.ToArray();
    }

    #endregion
}

#region Data Models

public class BasicProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class AdvancedDashboard
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DashboardData Data { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public object Metrics { get; set; } = new();
    public object[] Alerts { get; set; } = Array.Empty<object>();
}

public class DashboardData
{
    public DashboardMetrics Metrics { get; set; } = new();
    public string[] RecentActivity { get; set; } = Array.Empty<string>();
    public string[] Alerts { get; set; } = Array.Empty<string>();
}

public class DashboardMetrics
{
    public int Requests { get; set; }
    public int Errors { get; set; }
    public double Uptime { get; set; }
    public double ResponseTime { get; set; }
}

public class AuditEvent
{
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public Dictionary<string, object>? Metadata { get; set; }
}

public class AdvancedAuditLog
{
    public AuditEvent Event { get; set; } = new();
    public AuditContext Context { get; set; } = new();
}

public class AuditContext
{
    public string? CorrelationId { get; set; }
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string RequestPath { get; set; } = string.Empty;
    public string RequestMethod { get; set; } = string.Empty;
}

public class AdvancedSecurityStatus
{
    public Dictionary<string, string> SecurityHeaders { get; set; } = new();
    public string CspNonce { get; set; } = string.Empty;
    public Dictionary<string, object> CircuitBreakerStatus { get; set; } = new();
    public SecurityMetrics SecurityMetrics { get; set; } = new();
}

public class SecurityMetrics
{
    public DateTime LastSecurityScan { get; set; }
    public int VulnerabilitiesFound { get; set; }
    public int SecurityScore { get; set; }
    public string ComplianceStatus { get; set; } = string.Empty;
}

public class TenantData
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class ResilienceOptions
{
    public int RetryCount { get; set; } = 0;
    public int RetryDelay { get; set; } = 1000; // milliseconds
    public object? FallbackValue { get; set; }
}

public class ResilienceException : Exception
{
    public ResilienceException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class BaseContent
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string[] Media { get; set; } = Array.Empty<string>();
}

public class AdaptiveContent
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public object? Media { get; set; }
    public object? Layout { get; set; }
    public string[]? Features { get; set; }
    public string OptimizedFor { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public bool TouchSupport { get; set; }
}

#endregion
