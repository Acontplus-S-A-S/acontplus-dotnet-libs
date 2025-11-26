using Acontplus.Core.Abstractions.Services;
using Acontplus.Services.Services.Abstractions;
using Asp.Versioning;
using System.ComponentModel.DataAnnotations;

namespace Acontplus.TestApi.Endpoints.Api;

/// <summary>
/// Extension methods for mapping advanced service demonstration endpoints.
/// </summary>
public static class AdvancedEndpoints
{
    /// <summary>
    /// Maps all advanced demonstration endpoints.
    /// </summary>
    public static void MapAdvancedEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var apiVersionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .ReportApiVersions()
            .Build();

        // Version 1.0 endpoints
        var v1Group = endpoints.MapGroup("/api/v{version:apiVersion}/advanced-minimal")
            .WithTags("Advanced Minimal APIs v1.0")
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1, 0)
            .WithOpenApi();

        // Basic context endpoint
        v1Group.MapGet("/context", GetRequestContext)
            .WithName("GetRequestContextMinimalV1")
            .WithSummary("Get request context information using minimal API (v1.0)");

        // Device detection endpoint
        v1Group.MapGet("/device", GetDeviceInfo)
            .WithName("GetDeviceInfoMinimalV1")
            .WithSummary("Get device detection information using minimal API (v1.0)");

        // Version 2.0 endpoints
        var v2Group = endpoints.MapGroup("/api/v{version:apiVersion}/advanced-minimal")
            .WithTags("Advanced Minimal APIs v2.0")
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(2, 0)
            .WithOpenApi();

        // Enhanced context endpoint with additional data
        v2Group.MapGet("/context", GetRequestContextV2)
            .WithName("GetRequestContextMinimalV2")
            .WithSummary("Get enhanced request context information using minimal API (v2.0)");

        // Enhanced device detection endpoint
        v2Group.MapGet("/device", GetDeviceInfoV2)
            .WithName("GetDeviceInfoMinimalV2")
            .WithSummary("Get enhanced device detection information using minimal API (v2.0)");

        // New endpoint only available in v2.0
        v2Group.MapGet("/analytics", GetAnalytics)
            .WithName("GetAnalyticsMinimalV2")
            .WithSummary("Get analytics data (v2.0 only)");

        // Secure endpoint with client ID requirement (available in both versions)
        v1Group.MapGet("/secure", GetSecureData)
            .RequireAuthorization("RequireClientId")
            .WithName("GetSecureDataMinimalV1")
            .WithSummary("Secure endpoint requiring client ID (v1.0)");

        v2Group.MapGet("/secure", GetSecureDataV2)
            .RequireAuthorization("RequireClientId")
            .WithName("GetSecureDataMinimalV2")
            .WithSummary("Enhanced secure endpoint requiring client ID (v2.0)");

        // Tenant-specific endpoint
        v1Group.MapGet("/tenant/{tenantId}", GetTenantSpecificData)
            .RequireAuthorization("RequireTenant")
            .WithName("GetTenantSpecificDataMinimalV1")
            .WithSummary("Get tenant-specific data (v1.0)");

        v2Group.MapGet("/tenant/{tenantId}", GetTenantSpecificDataV2)
            .RequireAuthorization("RequireTenant")
            .WithName("GetTenantSpecificDataMinimalV2")
            .WithSummary("Get enhanced tenant-specific data (v2.0)");

        // Mobile-only endpoint
        v1Group.MapGet("/mobile-features", GetMobileFeatures)
            .RequireAuthorization("MobileOnly")
            .WithName("GetMobileFeaturesMinimalV1")
            .WithSummary("Mobile-only features endpoint (v1.0)");

        v2Group.MapGet("/mobile-features", GetMobileFeaturesV2)
            .RequireAuthorization("MobileOnly")
            .WithName("GetMobileFeaturesMinimalV2")
            .WithSummary("Enhanced mobile-only features endpoint (v2.0)");

        // Health check endpoint
        v1Group.MapGet("/health-summary", GetHealthSummary)
            .WithName("GetHealthSummaryMinimalV1")
            .WithSummary("Get advanced services health summary (v1.0)");

        v2Group.MapGet("/health-summary", GetHealthSummaryV2)
            .WithName("GetHealthSummaryMinimalV2")
            .WithSummary("Get enhanced advanced services health summary (v2.0)");

        // Validation endpoint
        v1Group.MapPost("/validate", ValidateDataMinimal)
            .WithName("ValidateDataMinimalV1")
            .WithSummary("Validate data using minimal API (v1.0)");

        v2Group.MapPost("/validate", ValidateDataMinimalV2)
            .WithName("ValidateDataMinimalV2")
            .WithSummary("Enhanced data validation using minimal API (v2.0)");
    }

    private static IResult GetRequestContext(
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var context = requestContext.GetRequestContext();
            var correlationId = requestContext.GetCorrelationId();

            logger.LogInformation("Minimal API: Request context retrieved for {CorrelationId}", correlationId);

            return Results.Ok(new
            {
                Context = context,
                ApiType = "Minimal API",
                Message = "Request context retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving request context in minimal API");
            return Results.Problem("Error retrieving request context");
        }
    }

    private static IResult GetDeviceInfo(
        IDeviceDetectionService deviceDetection,
        HttpContext httpContext)
    {
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var capabilities = deviceDetection.GetDeviceCapabilities(userAgent);
        var deviceType = deviceDetection.DetectDeviceType(httpContext);

        return Results.Ok(new
        {
            DeviceType = deviceType.ToString(),
            Capabilities = capabilities,
            ApiType = "Minimal API",
            Message = "Device information detected successfully"
        });
    }

    private static IResult GetSecureData(
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();
            var clientId = requestContext.GetClientId();

            logger.LogInformation("Minimal API: Secure data accessed by client: {ClientId}, correlation: {CorrelationId}",
                clientId, correlationId);

            return Results.Ok(new
            {
                Message = "Access granted to secure minimal API endpoint",
                ClientId = clientId,
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error accessing secure data in minimal API");
            return Results.Problem("Error accessing secure data");
        }
    }

    private static IResult GetTenantSpecificData(
        string tenantId,
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();

            logger.LogInformation("Minimal API: Tenant data requested for tenant: {TenantId}, correlation: {CorrelationId}",
                tenantId, correlationId);

            return Results.Ok(new
            {
                Message = "Tenant-specific data retrieved successfully",
                TenantId = tenantId,
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Data = new
                {
                    TenantName = $"Tenant {tenantId}",
                    Settings = new { Theme = "Light", Language = "es-EC" },
                    LastUpdated = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tenant data in minimal API");
            return Results.Problem("Error retrieving tenant data");
        }
    }

    private static IResult GetMobileFeatures(
        IDeviceDetectionService deviceDetection,
        HttpContext httpContext,
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var deviceType = deviceDetection.DetectDeviceType(httpContext);
            var correlationId = requestContext.GetCorrelationId();

            logger.LogInformation("Minimal API: Mobile features requested for device: {DeviceType}, correlation: {CorrelationId}",
                deviceType, correlationId);

            return Results.Ok(new
            {
                Message = "Mobile features retrieved successfully",
                DeviceType = deviceType.ToString(),
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Features = new
                {
                    TouchOptimized = true,
                    ResponsiveDesign = true,
                    MobileNotifications = true,
                    OfflineSupport = true,
                    GestureSupport = true
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving mobile features in minimal API");
            return Results.Problem("Error retrieving mobile features");
        }
    }

    private static IResult GetHealthSummary(
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();
            var timestamp = DateTime.UtcNow;

            logger.LogInformation("Minimal API: Health summary requested for correlation: {CorrelationId}", correlationId);

            return Results.Ok(new
            {
                Status = "Healthy",
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Timestamp = timestamp,
                Message = "Advanced services health summary",
                Services = new
                {
                    RequestContext = "Healthy",
                    DeviceDetection = "Healthy",
                    SecurityHeaders = "Healthy",
                    Authentication = "Healthy"
                },
                Metrics = new
                {
                    ResponseTime = "45ms",
                    Uptime = "99.9%",
                    ActiveConnections = 1250
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving health summary in minimal API");
            return Results.Problem("Error retrieving health summary");
        }
    }

    private static IResult ValidateDataMinimal(
        [FromBody] EndpointValidationRequest request,
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();
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

            logger.LogInformation("Minimal API: Data validation completed for correlation: {CorrelationId}, results: {ValidationResults}",
                correlationId, validationResults.Count);

            if (validationResults.Any())
            {
                return Results.BadRequest(new
                {
                    Message = "Validation failed",
                    CorrelationId = correlationId,
                    ApiType = "Minimal API",
                    Version = "1.0",
                    Errors = validationResults.Select(v => new
                    {
                        Field = v.MemberNames.FirstOrDefault(),
                        Message = v.ErrorMessage
                    })
                });
            }

            return Results.Ok(new
            {
                Message = "Data validation successful",
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Version = "1.0",
                ValidatedData = request
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating data in minimal API");
            return Results.Problem("Error validating data");
        }
    }

    // Version 2.0 endpoint implementations

    private static IResult GetRequestContextV2(
        IRequestContextService requestContext,
        HttpContext httpContext,
        ILogger<Program> logger)
    {
        try
        {
            var context = requestContext.GetRequestContext();
            var correlationId = requestContext.GetCorrelationId();
            var clientId = requestContext.GetClientId();
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            logger.LogInformation("Minimal API v2.0: Enhanced request context retrieved for {CorrelationId}", correlationId);

            return Results.Ok(new
            {
                Context = context,
                CorrelationId = correlationId,
                ClientId = clientId,
                UserAgent = userAgent,
                ApiType = "Minimal API",
                Version = "2.0",
                Message = "Enhanced request context retrieved successfully",
                Timestamp = DateTime.UtcNow,
                ServerInfo = new
                {
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    MachineName = Environment.MachineName,
                    ProcessId = Environment.ProcessId
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving enhanced request context in minimal API v2.0");
            return Results.Problem("Error retrieving enhanced request context");
        }
    }

    private static IResult GetDeviceInfoV2(
        IDeviceDetectionService deviceDetection,
        HttpContext httpContext,
        IRequestContextService requestContext)
    {
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var capabilities = deviceDetection.GetDeviceCapabilities(userAgent);
        var deviceType = deviceDetection.DetectDeviceType(httpContext);
        var correlationId = requestContext.GetCorrelationId();

        return Results.Ok(new
        {
            DeviceType = deviceType.ToString(),
            Capabilities = capabilities,
            CorrelationId = correlationId,
            ApiType = "Minimal API",
            Version = "2.0",
            Message = "Enhanced device information detected successfully",
            RequestInfo = new
            {
                UserAgent = userAgent,
                RemoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                IsHttps = httpContext.Request.IsHttps,
                Protocol = httpContext.Request.Protocol
            }
        });
    }

    private static IResult GetAnalytics(
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();
            var timestamp = DateTime.UtcNow;

            logger.LogInformation("Minimal API v2.0: Analytics requested for correlation: {CorrelationId}", correlationId);

            return Results.Ok(new
            {
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Version = "2.0",
                Timestamp = timestamp,
                Message = "Analytics data retrieved successfully",
                Analytics = new
                {
                    TotalRequests = 15420,
                    AverageResponseTime = "45ms",
                    ErrorRate = "0.02%",
                    TopEndpoints = new[]
                    {
                        new { Endpoint = "/api/v2/advanced-minimal/context", Count = 1250 },
                        new { Endpoint = "/api/v2/advanced-minimal/device", Count = 980 },
                        new { Endpoint = "/api/v2/advanced-minimal/health-summary", Count = 750 }
                    },
                    PerformanceMetrics = new
                    {
                        CpuUsage = "15.2%",
                        MemoryUsage = "256MB",
                        ActiveConnections = 42
                    }
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving analytics in minimal API v2.0");
            return Results.Problem("Error retrieving analytics data");
        }
    }

    private static IResult GetSecureDataV2(
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();
            var clientId = requestContext.GetClientId();
            var timestamp = DateTime.UtcNow;

            logger.LogInformation("Minimal API v2.0: Enhanced secure data accessed by client: {ClientId}, correlation: {CorrelationId}",
                clientId, correlationId);

            return Results.Ok(new
            {
                Message = "Enhanced access granted to secure minimal API endpoint",
                ClientId = clientId,
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Version = "2.0",
                Timestamp = timestamp,
                SecurityInfo = new
                {
                    AuthenticationMethod = "JWT Bearer",
                    AuthorizationPolicies = new[] { "RequireClientId" },
                    EncryptionLevel = "AES-256",
                    TokenExpiry = timestamp.AddHours(1)
                },
                Data = new
                {
                    SensitiveValue = "ENCRYPTED_DATA_V2",
                    AccessLevel = "Premium",
                    Features = new[] { "AdvancedAnalytics", "RealTimeUpdates", "PrioritySupport" }
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error accessing enhanced secure data in minimal API v2.0");
            return Results.Problem("Error accessing enhanced secure data");
        }
    }

    private static IResult GetTenantSpecificDataV2(
        string tenantId,
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();

            logger.LogInformation("Minimal API v2.0: Enhanced tenant data requested for tenant: {TenantId}, correlation: {CorrelationId}",
                tenantId, correlationId);

            return Results.Ok(new
            {
                Message = "Enhanced tenant-specific data retrieved successfully",
                TenantId = tenantId,
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Version = "2.0",
                Timestamp = DateTime.UtcNow,
                TenantInfo = new
                {
                    Name = $"Tenant {tenantId}",
                    Plan = "Enterprise",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow.AddDays(-365),
                    Settings = new
                    {
                        Theme = "Dark",
                        Language = "es-EC",
                        Timezone = "America/Guayaquil",
                        Features = new[] { "AdvancedReporting", "MultiTenant", "ApiAccess" }
                    },
                    Usage = new
                    {
                        ApiCalls = 15420,
                        StorageUsed = "2.5GB",
                        ActiveUsers = 150
                    }
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving enhanced tenant data in minimal API v2.0");
            return Results.Problem("Error retrieving enhanced tenant data");
        }
    }

    private static IResult GetMobileFeaturesV2(
        IDeviceDetectionService deviceDetection,
        HttpContext httpContext,
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var deviceType = deviceDetection.DetectDeviceType(httpContext);
            var correlationId = requestContext.GetCorrelationId();
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            logger.LogInformation("Minimal API v2.0: Enhanced mobile features requested for device: {DeviceType}, correlation: {CorrelationId}",
                deviceType, correlationId);

            return Results.Ok(new
            {
                Message = "Enhanced mobile features retrieved successfully",
                DeviceType = deviceType.ToString(),
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Version = "2.0",
                Timestamp = DateTime.UtcNow,
                DeviceInfo = new
                {
                    UserAgent = userAgent,
                    ScreenSize = "Responsive",
                    TouchEnabled = true,
                    GpsEnabled = true
                },
                Features = new
                {
                    TouchOptimized = true,
                    ResponsiveDesign = true,
                    MobileNotifications = true,
                    OfflineSupport = true,
                    GestureSupport = true,
                    PushNotifications = true,
                    BiometricAuth = true,
                    LocationServices = true,
                    CameraAccess = true
                },
                Performance = new
                {
                    OptimizedForMobile = true,
                    CompressedResponses = true,
                    LazyLoading = true
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving enhanced mobile features in minimal API v2.0");
            return Results.Problem("Error retrieving enhanced mobile features");
        }
    }

    private static IResult GetHealthSummaryV2(
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();
            var timestamp = DateTime.UtcNow;

            logger.LogInformation("Minimal API v2.0: Enhanced health summary requested for correlation: {CorrelationId}", correlationId);

            return Results.Ok(new
            {
                Status = "Healthy",
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Version = "2.0",
                Timestamp = timestamp,
                Message = "Enhanced advanced services health summary",
                Services = new
                {
                    RequestContext = new { Status = "Healthy", ResponseTime = "5ms", Uptime = "99.9%" },
                    DeviceDetection = new { Status = "Healthy", ResponseTime = "8ms", Uptime = "99.8%" },
                    SecurityHeaders = new { Status = "Healthy", ResponseTime = "2ms", Uptime = "100%" },
                    Authentication = new { Status = "Healthy", ResponseTime = "12ms", Uptime = "99.7%" },
                    Caching = new { Status = "Healthy", ResponseTime = "3ms", Uptime = "99.9%" },
                    Database = new { Status = "Healthy", ResponseTime = "15ms", Uptime = "99.5%" }
                },
                Metrics = new
                {
                    ResponseTime = "42ms",
                    Uptime = "99.9%",
                    ActiveConnections = 1250,
                    TotalRequests = 15420,
                    ErrorRate = "0.02%",
                    Throughput = "150 req/sec"
                },
                System = new
                {
                    CpuUsage = "15.2%",
                    MemoryUsage = "256MB",
                    DiskUsage = "45%",
                    NetworkLatency = "2ms"
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving enhanced health summary in minimal API v2.0");
            return Results.Problem("Error retrieving enhanced health summary");
        }
    }

    private static IResult ValidateDataMinimalV2(
        [FromBody] EndpointValidationRequestV2 request,
        IRequestContextService requestContext,
        ILogger<Program> logger)
    {
        try
        {
            var correlationId = requestContext.GetCorrelationId();
            var validationResults = new List<ValidationResult>();

            // Enhanced validation for v2.0
            if (string.IsNullOrWhiteSpace(request.Name))
                validationResults.Add(new ValidationResult("Name is required", new[] { "Name" }));

            if (request.Age < 0 || request.Age > 150)
                validationResults.Add(new ValidationResult("Age must be between 0 and 150", new[] { "Age" }));

            if (string.IsNullOrWhiteSpace(request.Email))
                validationResults.Add(new ValidationResult("Email is required", new[] { "Email" }));

            // Enhanced email validation
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (!request.Email.Contains("@"))
                    validationResults.Add(new ValidationResult("Invalid email format", new[] { "Email" }));
                else if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    validationResults.Add(new ValidationResult("Invalid email format", new[] { "Email" }));
            }

            // New validation for phone
            if (!string.IsNullOrWhiteSpace(request.Phone) && !System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^\+?[\d\s\-\(\)]+$"))
                validationResults.Add(new ValidationResult("Invalid phone number format", new[] { "Phone" }));

            logger.LogInformation("Minimal API v2.0: Enhanced data validation completed for correlation: {CorrelationId}, results: {ValidationResults}",
                correlationId, validationResults.Count);

            if (validationResults.Any())
            {
                return Results.BadRequest(new
                {
                    Message = "Validation failed",
                    CorrelationId = correlationId,
                    ApiType = "Minimal API",
                    Version = "2.0",
                    Timestamp = DateTime.UtcNow,
                    Errors = validationResults.Select(v => new
                    {
                        Field = v.MemberNames.FirstOrDefault(),
                        Message = v.ErrorMessage,
                        Severity = "Error"
                    }),
                    ValidationSummary = new
                    {
                        TotalErrors = validationResults.Count,
                        FieldsValidated = new[] { "Name", "Age", "Email", "Phone" }
                    }
                });
            }

            return Results.Ok(new
            {
                Message = "Enhanced data validation successful",
                CorrelationId = correlationId,
                ApiType = "Minimal API",
                Version = "2.0",
                Timestamp = DateTime.UtcNow,
                ValidatedData = request,
                ValidationSummary = new
                {
                    TotalErrors = 0,
                    FieldsValidated = new[] { "Name", "Age", "Email", "Phone" },
                    ValidationTime = "5ms"
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating enhanced data in minimal API v2.0");
            return Results.Problem("Error validating enhanced data");
        }
    }
}

public class EndpointValidationRequest
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class EndpointValidationRequestV2
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

