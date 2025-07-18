using Acontplus.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Acontplus.TestApi.Controllers;

/// <summary>
/// Controller demonstrating enterprise service patterns and features.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EnterpriseExamplesController : ControllerBase
{
    private readonly IRequestContextService _requestContext;
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly ISecurityHeaderService _securityHeaders;
    private readonly ILogger<EnterpriseExamplesController> _logger;

    public EnterpriseExamplesController(
        IRequestContextService requestContext,
        IDeviceDetectionService deviceDetection,
        ISecurityHeaderService securityHeaders,
        ILogger<EnterpriseExamplesController> logger)
    {
        _requestContext = requestContext;
        _deviceDetection = deviceDetection;
        _securityHeaders = securityHeaders;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrates request context service usage.
    /// </summary>
    [HttpGet("context")]
    public IActionResult GetRequestContext()
    {
        var context = _requestContext.GetRequestContext();
        var isMobile = _requestContext.IsMobileRequest();
        var deviceType = _requestContext.GetDeviceType();

        _logger.LogInformation("Request context retrieved for correlation ID: {CorrelationId}",
            _requestContext.GetCorrelationId());

        return Ok(new
        {
            Context = context,
            IsMobile = isMobile,
            DeviceType = deviceType.ToString(),
            Message = "Request context successfully retrieved"
        });
    }

    /// <summary>
    /// Demonstrates device detection capabilities.
    /// </summary>
    [HttpGet("device")]
    public IActionResult GetDeviceInfo()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var capabilities = _deviceDetection.GetDeviceCapabilities(userAgent);
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        var isMobile = _deviceDetection.IsMobileDevice(HttpContext);

        return Ok(new
        {
            UserAgent = userAgent,
            DetectedType = deviceType.ToString(),
            IsMobile = isMobile,
            Capabilities = capabilities,
            Message = "Device information successfully detected"
        });
    }

    /// <summary>
    /// Demonstrates security headers service.
    /// </summary>
    [HttpGet("security-headers")]
    public IActionResult GetSecurityHeaders()
    {
        var isDevelopment = HttpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>().IsDevelopment();

        var recommendedHeaders = _securityHeaders.GetRecommendedHeaders(isDevelopment);
        var nonce = _securityHeaders.GenerateCspNonce();

        return Ok(new
        {
            RecommendedHeaders = recommendedHeaders,
            CspNonce = nonce,
            Environment = isDevelopment ? "Development" : "Production",
            Message = "Security headers information retrieved"
        });
    }

    /// <summary>
    /// Endpoint that requires client ID validation.
    /// </summary>
    [HttpGet("secure")]
    [Authorize(Policy = "RequireClientId")]
    public IActionResult SecureEndpoint()
    {
        var clientId = _requestContext.GetClientId();

        return Ok(new
        {
            ClientId = clientId,
            Message = "Access granted to secure endpoint",
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Endpoint that requires tenant isolation.
    /// </summary>
    [HttpGet("tenant-data")]
    [Authorize(Policy = "RequireTenant")]
    public IActionResult GetTenantData()
    {
        var tenantId = _requestContext.GetTenantId();

        return Ok(new
        {
            TenantId = tenantId,
            Data = $"Tenant-specific data for {tenantId}",
            Message = "Tenant data retrieved successfully"
        });
    }

    /// <summary>
    /// Mobile-only endpoint demonstrating device-based authorization.
    /// </summary>
    [HttpGet("mobile-only")]
    [Authorize(Policy = "MobileOnly")]
    public IActionResult MobileOnlyEndpoint()
    {
        var deviceType = _requestContext.GetDeviceType();

        return Ok(new
        {
            DeviceType = deviceType.ToString(),
            Message = "Mobile-only access granted",
            Features = new[] { "Touch optimized", "Offline sync", "Push notifications" }
        });
    }

    /// <summary>
    /// Endpoint for desktop and web access only.
    /// </summary>
    [HttpGet("desktop-only")]
    [Authorize(Policy = "DesktopOnly")]
    public IActionResult DesktopOnlyEndpoint()
    {
        return Ok(new
        {
            Message = "Desktop access granted",
            Features = new[] { "Full keyboard support", "Multi-window", "Advanced reporting" }
        });
    }

    /// <summary>
    /// Demonstrates validation filter in action.
    /// </summary>
    [HttpPost("validate")]
    public IActionResult ValidateData([FromBody] TestDataModel model)
    {
        // ValidationActionFilter will automatically handle model validation
        return Ok(new
        {
            Message = "Data validation passed",
            ReceivedData = model
        });
    }

    /// <summary>
    /// Endpoint that intentionally throws an exception to demonstrate global exception handling.
    /// </summary>
    [HttpGet("test-exception")]
    public IActionResult TestException()
    {
        throw new InvalidOperationException("This is a test exception to demonstrate global exception handling");
    }
}

/// <summary>
/// Test model for validation demonstration.
/// </summary>
public class TestDataModel
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Range(18, 120)]
    public int Age { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}