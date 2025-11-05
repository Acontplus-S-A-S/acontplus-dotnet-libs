using Acontplus.Services.Services.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Acontplus.TestApi.Endpoints;

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
        var group = endpoints.MapGroup("/api/advanced-minimal")
            .WithTags("Advanced Minimal APIs")
            .WithOpenApi();

        // Basic context endpoint
        group.MapGet("/context", GetRequestContext)
            .WithName("GetRequestContextMinimal")
            .WithSummary("Get request context information using minimal API");

        // Device detection endpoint
        group.MapGet("/device", GetDeviceInfo)
            .WithName("GetDeviceInfoMinimal")
            .WithSummary("Get device detection information using minimal API");

        // Secure endpoint with client ID requirement
        group.MapGet("/secure", GetSecureData)
            .RequireAuthorization("RequireClientId")
            .WithName("GetSecureDataMinimal")
            .WithSummary("Secure endpoint requiring client ID");

        // Tenant-specific endpoint
        group.MapGet("/tenant/{tenantId}", GetTenantSpecificData)
            .RequireAuthorization("RequireTenant")
            .WithName("GetTenantSpecificDataMinimal")
            .WithSummary("Get tenant-specific data");

        // Mobile-only endpoint
        group.MapGet("/mobile-features", GetMobileFeatures)
            .RequireAuthorization("MobileOnly")
            .WithName("GetMobileFeaturesMinimal")
            .WithSummary("Mobile-only features endpoint");

        // Health check endpoint
        group.MapGet("/health-summary", GetHealthSummary)
            .WithName("GetHealthSummaryMinimal")
            .WithSummary("Get advanced services health summary");

        // Validation endpoint
        group.MapPost("/validate", ValidateDataMinimal)
            .WithName("ValidateDataMinimal")
            .WithSummary("Validate data using minimal API");
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
                ValidatedData = request
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating data in minimal API");
            return Results.Problem("Error validating data");
        }
    }
}

public class EndpointValidationRequest
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}
