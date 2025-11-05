using Acontplus.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Acontplus.TestApi.Controllers;

/// <summary>
/// Controller demonstrating advanced service patterns and features.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AdvancedExamplesController : ControllerBase
{
    private readonly IRequestContextService _requestContext;
    private readonly IDeviceDetectionService _deviceDetection;
    private readonly ISecurityHeaderService _securityHeaders;
    private readonly ILogger<AdvancedExamplesController> _logger;

    public AdvancedExamplesController(
        IRequestContextService requestContext,
        IDeviceDetectionService deviceDetection,
        ISecurityHeaderService securityHeaders,
        ILogger<AdvancedExamplesController> logger)
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
        var clientId = User.FindFirst("client_id")?.Value;
        var correlationId = _requestContext.GetCorrelationId();

        _logger.LogInformation("Secure endpoint accessed by client: {ClientId}, correlation: {CorrelationId}",
            clientId, correlationId);

        return Ok(new
        {
            Message = "Access granted to secure endpoint",
            ClientId = clientId,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Demonstrates tenant isolation with multi-tenant support.
    /// </summary>
    [HttpGet("tenant-data")]
    [Authorize(Policy = "RequireTenant")]
    public IActionResult GetTenantData()
    {
        var tenantId = User.FindFirst("tenant_id")?.Value;
        var correlationId = _requestContext.GetCorrelationId();

        _logger.LogInformation("Tenant data requested for tenant: {TenantId}, correlation: {CorrelationId}",
            tenantId, correlationId);

        return Ok(new
        {
            Message = "Tenant data retrieved successfully",
            TenantId = tenantId,
            CorrelationId = correlationId,
            Data = new
            {
                TenantName = $"Tenant {tenantId}",
                Settings = new { Theme = "Dark", Language = "en-US" },
                LastUpdated = DateTime.UtcNow
            }
        });
    }

    /// <summary>
    /// Demonstrates mobile-specific features and policies.
    /// </summary>
    [HttpGet("mobile-only")]
    [Authorize(Policy = "MobileOnly")]
    public IActionResult GetMobileFeatures()
    {
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        var correlationId = _requestContext.GetCorrelationId();

        _logger.LogInformation("Mobile features requested for device: {DeviceType}, correlation: {CorrelationId}",
            deviceType, correlationId);

        return Ok(new
        {
            Message = "Mobile features retrieved successfully",
            DeviceType = deviceType.ToString(),
            CorrelationId = correlationId,
            Features = new
            {
                TouchOptimized = true,
                ResponsiveDesign = true,
                MobileNotifications = true,
                OfflineSupport = true
            }
        });
    }

    /// <summary>
    /// Demonstrates desktop-specific features and policies.
    /// </summary>
    [HttpGet("desktop-only")]
    [Authorize(Policy = "DesktopOnly")]
    public IActionResult GetDesktopFeatures()
    {
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        var correlationId = _requestContext.GetCorrelationId();

        _logger.LogInformation("Desktop features requested for device: {DeviceType}, correlation: {CorrelationId}",
            deviceType, correlationId);

        return Ok(new
        {
            Message = "Desktop features retrieved successfully",
            DeviceType = deviceType.ToString(),
            CorrelationId = correlationId,
            Features = new
            {
                FullScreenMode = true,
                KeyboardShortcuts = true,
                MultiWindowSupport = true,
                AdvancedAnalytics = true
            }
        });
    }

    /// <summary>
    /// Demonstrates comprehensive data validation with multiple validators.
    /// </summary>
    [HttpPost("validate")]
    public IActionResult ValidateData([FromBody] ControllerValidationRequest request)
    {
        var correlationId = _requestContext.GetCorrelationId();
        var validationResults = new List<ValidationResult>();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Name))
            validationResults.Add(new ValidationResult("Name is required", new[] { "Name" }));

        if (request.Age < 0 || request.Age > 150)
            validationResults.Add(new ValidationResult("Age must be between 0 and 150", new[] { "Age" }));

        if (string.IsNullOrWhiteSpace(request.Email))
            validationResults.Add(new ValidationResult("Email is required", new[] { "Email" }));

        // Validate email format
        if (!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Contains("@"))
            validationResults.Add(new ValidationResult("Invalid email format", new[] { "Email" }));

        _logger.LogInformation("Data validation completed for correlation: {CorrelationId}, results: {ValidationResults}",
            correlationId, validationResults.Count);

        if (validationResults.Any())
        {
            return BadRequest(new
            {
                Message = "Validation failed",
                CorrelationId = correlationId,
                Errors = validationResults.Select(v => new
                {
                    Field = v.MemberNames.FirstOrDefault(),
                    Message = v.ErrorMessage
                })
            });
        }

        return Ok(new
        {
            Message = "Data validation successful",
            CorrelationId = correlationId,
            ValidatedData = request
        });
    }

    #region Advanced Usage Examples

    /// <summary>
    /// Advanced usage - Multi-tenant dashboard with comprehensive monitoring.
    /// </summary>
    [HttpGet("advanced/dashboard")]
    [Authorize(Policy = "RequireTenant")]
    public async Task<IActionResult> GetAdvancedDashboard()
    {
        var tenantId = User.FindFirst("tenant_id")?.Value;
        var correlationId = _requestContext.GetCorrelationId();
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);

        _logger.LogInformation("Advanced dashboard requested for tenant: {TenantId}, device: {DeviceType}, correlation: {CorrelationId}",
            tenantId, deviceType, correlationId);

        // Simulate async operation
        await Task.Delay(100);

        return Ok(new AdvancedDashboard
        {
            TenantId = tenantId,
            CorrelationId = correlationId,
            DeviceType = deviceType.ToString(),
            Timestamp = DateTime.UtcNow,
            Metrics = new
            {
                ActiveUsers = 1250,
                ResponseTime = 45.2,
                ErrorRate = 0.02,
                Throughput = 1500
            },
            Alerts = new[]
            {
                new { Type = "Info", Message = "System operating normally", Severity = "Low" },
                new { Type = "Warning", Message = "High memory usage detected", Severity = "Medium" }
            }
        });
    }

    /// <summary>
    /// Advanced usage - Audit logging with comprehensive context.
    /// </summary>
    [HttpPost("advanced/audit")]
    public async Task<IActionResult> CreateAdvancedAuditLog([FromBody] AuditLogRequest request)
    {
        var correlationId = _requestContext.GetCorrelationId();
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);
        var isMobile = _deviceDetection.IsMobileDevice(HttpContext);

        _logger.LogInformation("Advanced audit log created for correlation: {CorrelationId}, device: {DeviceType}, mobile: {IsMobile}",
            correlationId, deviceType, isMobile);

        // Simulate async operation
        await Task.Delay(100);

        var auditLog = new AdvancedAuditLog
        {
            Event = new AuditEvent
            {
                Action = request.Action,
                Resource = request.Resource,
                Metadata = new Dictionary<string, object>
                {
                    ["IpAddress"] = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                    ["UserAgent"] = Request.Headers.UserAgent.ToString(),
                    ["RequestPath"] = Request.Path.Value ?? "",
                    ["RequestMethod"] = Request.Method
                }
            },
            Context = new AuditContext
            {
                CorrelationId = correlationId,
                TenantId = null,
                ClientId = null,
                UserId = request.UserId,
                Timestamp = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                RequestPath = Request.Path.Value ?? "",
                RequestMethod = Request.Method
            }
        };

        return Ok(new
        {
            Message = "Advanced audit log created successfully",
            AuditLog = auditLog
        });
    }

    /// <summary>
    /// Advanced usage - Security status and monitoring.
    /// </summary>
    [HttpGet("advanced/security-status")]
    public IActionResult GetAdvancedSecurityStatus()
    {
        var correlationId = _requestContext.GetCorrelationId();
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);

        _logger.LogInformation("Advanced security status requested for device: {DeviceType}, correlation: {CorrelationId}",
            deviceType, correlationId);

        return Ok(new AdvancedSecurityStatus
        {
            SecurityHeaders = new Dictionary<string, string>
            {
                ["X-Frame-Options"] = "DENY",
                ["X-Content-Type-Options"] = "nosniff",
                ["X-XSS-Protection"] = "1; mode=block"
            },
            CspNonce = "nonce-" + Guid.NewGuid().ToString("N"),
            CircuitBreakerStatus = new Dictionary<string, object>
            {
                ["Status"] = "Closed",
                ["FailureCount"] = 0,
                ["LastFailureTime"] = DateTime.UtcNow.AddHours(-1)
            },
            SecurityMetrics = new SecurityMetrics
            {
                LastSecurityScan = DateTime.UtcNow.AddMinutes(-30),
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
    public async Task<IActionResult> GetAdvancedTenantData(string dataType)
    {
        var tenantId = User.FindFirst("tenant_id")?.Value;
        var correlationId = _requestContext.GetCorrelationId();
        var deviceType = _deviceDetection.DetectDeviceType(HttpContext);

        _logger.LogInformation("Advanced tenant data requested for tenant: {TenantId}, type: {DataType}, device: {DeviceType}, correlation: {CorrelationId}",
            tenantId, dataType, deviceType, correlationId);

        // Simulate async operation
        await Task.Delay(100);

        var data = new
        {
            TenantId = tenantId,
            DataType = dataType,
            DeviceType = deviceType.ToString(),
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Content = $"Advanced tenant-specific {dataType} for {tenantId}",
            Metadata = new
            {
                AccessLevel = "advanced",
                DataVersion = "2.0",
                LastModified = DateTime.UtcNow.AddHours(-2),
                ModifiedBy = "system"
            }
        };

        return Ok(data);
    }

    #endregion
}

public class ControllerValidationRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, 150)]
    public int Age { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class AuditLogRequest
{
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
