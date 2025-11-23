using Acontplus.Core.Abstractions.Infrastructure.Caching;
using Acontplus.Core.Abstractions.Services;
using Acontplus.Services.Services.Abstractions;

namespace Acontplus.TestApi.Controllers.Infrastructure;

/// <summary>
/// Controller to test Acontplus.Services configuration and injection.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigurationTestController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly IRequestContextService _requestContext;
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly ISecurityHeaderService _securityHeaders;
    private readonly ILogger<ConfigurationTestController> _logger;

    public ConfigurationTestController(
        ICacheService cache,
        IRequestContextService requestContext,
        IDeviceDetectionService deviceDetection,
        ISecurityHeaderService securityHeaders,
        ILogger<ConfigurationTestController> logger)
    {
        _cache = cache;
        _requestContext = requestContext;
        _deviceDetection = deviceDetection;
        _securityHeaders = securityHeaders;
        _logger = logger;
    }

    /// <summary>
    /// Test basic service injection and functionality.
    /// </summary>
    [HttpGet("services")]
    public IActionResult TestServices()
    {
        try
        {
            var correlationId = _requestContext.GetCorrelationId();
            var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
            var headers = _securityHeaders.GetRecommendedHeaders(false);

            return Ok(new
            {
                Message = "All Acontplus.Services are working correctly!",
                Services = new
                {
                    CacheService = _cache.GetType().Name,
                    RequestContextService = _requestContext.GetType().Name,
                    DeviceDetectionService = _deviceDetection.GetType().Name,
                    SecurityHeaderService = _securityHeaders.GetType().Name
                },
                TestResults = new
                {
                    CorrelationId = correlationId,
                    DeviceType = deviceType.ToString(),
                    SecurityHeadersCount = headers.Count,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing Acontplus.Services");
            return StatusCode(500, new { Error = "Services test failed", Details = ex.Message });
        }
    }

    /// <summary>
    /// Test cache service functionality.
    /// </summary>
    [HttpGet("cache")]
    public async Task<IActionResult> TestCache()
    {
        try
        {
            var testKey = "test-config-key";
            var testValue = $"Test value generated at {DateTime.UtcNow:HH:mm:ss}";

            // Test cache set
            await _cache.SetAsync(testKey, testValue, TimeSpan.FromMinutes(5));

            // Test cache get
            var retrievedValue = await _cache.GetAsync<string>(testKey);

            return Ok(new
            {
                Message = "Cache service test completed successfully",
                Test = new
                {
                    Key = testKey,
                    SetValue = testValue,
                    RetrievedValue = retrievedValue,
                    CacheHit = retrievedValue == testValue
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing cache service");
            return StatusCode(500, new { Error = "Cache test failed", Details = ex.Message });
        }
    }

    /// <summary>
    /// Test device detection service.
    /// </summary>
    [HttpGet("device")]
    public IActionResult TestDeviceDetection()
    {
        try
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
            var isMobile = _deviceDetection.IsMobileDevice(HttpContext);
            var capabilities = _deviceDetection.GetDeviceCapabilities(userAgent);

            return Ok(new
            {
                Message = "Device detection test completed successfully",
                Request = new
                {
                    UserAgent = userAgent,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                },
                Detection = new
                {
                    DeviceType = deviceType.ToString(),
                    IsMobile = isMobile,
                    Capabilities = capabilities
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing device detection service");
            return StatusCode(500, new { Error = "Device detection test failed", Details = ex.Message });
        }
    }

    /// <summary>
    /// Test security headers service.
    /// </summary>
    [HttpGet("security")]
    public IActionResult TestSecurityHeaders()
    {
        try
        {
            var isDevelopment = HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>().IsDevelopment();

            var headers = _securityHeaders.GetRecommendedHeaders(isDevelopment);
            var cspNonce = _securityHeaders.GenerateCspNonce();

            return Ok(new
            {
                Message = "Security headers test completed successfully",
                Environment = isDevelopment ? "Development" : "Production",
                SecurityHeaders = headers,
                CspNonce = cspNonce,
                NonceLength = cspNonce.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing security headers service");
            return StatusCode(500, new { Error = "Security headers test failed", Details = ex.Message });
        }
    }

    /// <summary>
    /// Test request context service.
    /// </summary>
    [HttpGet("context")]
    public IActionResult TestRequestContext()
    {
        try
        {
            var context = _requestContext.GetRequestContext();
            var correlationId = _requestContext.GetCorrelationId();
            var clientId = _requestContext.GetClientId();
            var tenantId = _requestContext.GetTenantId();

            return Ok(new
            {
                Message = "Request context test completed successfully",
                Context = new
                {
                    CorrelationId = correlationId,
                    ClientId = clientId,
                    TenantId = tenantId,
                    RequestId = _requestContext.GetRequestId(),
                    Timestamp = DateTime.UtcNow
                },
                Headers = new
                {
                    XClientId = Request.Headers["X-Client-ID"].ToString(),
                    XTenantId = Request.Headers["X-Tenant-ID"].ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing request context service");
            return StatusCode(500, new { Error = "Request context test failed", Details = ex.Message });
        }
    }
}

