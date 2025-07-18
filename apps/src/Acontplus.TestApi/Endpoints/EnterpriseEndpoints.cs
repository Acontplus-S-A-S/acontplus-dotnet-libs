using Acontplus.Services.Services.Abstractions;

namespace Acontplus.TestApi.Endpoints;

/// <summary>
/// Extension methods for mapping enterprise service demonstration endpoints.
/// </summary>
public static class EnterpriseEndpoints
{
    /// <summary>
    /// Maps all enterprise demonstration endpoints.
    /// </summary>
    public static void MapEnterpriseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/enterprise-minimal")
            .WithTags("Enterprise Minimal APIs")
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
            .WithSummary("Get enterprise services health summary");

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
        HttpContext httpContext)
    {
        var clientId = requestContext.GetClientId();
        var requestId = requestContext.GetRequestId();

        return Results.Ok(new
        {
            ClientId = clientId,
            RequestId = requestId,
            SecureData = "This is protected data",
            ApiType = "Minimal API",
            Message = "Secure data accessed successfully"
        });
    }

    private static IResult GetTenantSpecificData(
        string tenantId,
        IRequestContextService requestContext)
    {
        var contextTenantId = requestContext.GetTenantId();

        // Verify tenant ID matches
        if (!string.Equals(tenantId, contextTenantId, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Forbid();
        }

        return Results.Ok(new
        {
            TenantId = tenantId,
            Data = new
            {
                TenantName = $"Tenant {tenantId}",
                Settings = new { Theme = "Blue", Language = "en-US" },
                LastAccess = DateTime.UtcNow
            },
            ApiType = "Minimal API",
            Message = "Tenant-specific data retrieved"
        });
    }

    private static IResult GetMobileFeatures(
        IRequestContextService requestContext,
        IDeviceDetectionService deviceDetection,
        HttpContext httpContext)
    {
        var deviceType = requestContext.GetDeviceType();
        var capabilities = deviceDetection.GetDeviceCapabilities(
            httpContext.Request.Headers.UserAgent.ToString());

        return Results.Ok(new
        {
            DeviceType = deviceType.ToString(),
            MobileFeatures = new[]
            {
                "Touch gestures",
                "Camera integration",
                "GPS location",
                "Push notifications",
                "Offline sync"
            },
            DeviceCapabilities = capabilities,
            ApiType = "Minimal API",
            Message = "Mobile features accessed successfully"
        });
    }

    private static async Task<IResult> GetHealthSummary(
        IServiceProvider serviceProvider)
    {
        try
        {
            // Get services and check their health
            var requestContext = serviceProvider.GetRequiredService<IRequestContextService>();
            var deviceDetection = serviceProvider.GetRequiredService<IDeviceDetectionService>();
            var securityHeaders = serviceProvider.GetRequiredService<ISecurityHeaderService>();

            var healthSummary = new
            {
                Services = new
                {
                    RequestContext = await CheckServiceHealth(() => requestContext.GetRequestContext()),
                    DeviceDetection = await CheckServiceHealth(() => deviceDetection.GetDeviceCapabilities("test")),
                    SecurityHeaders = await CheckServiceHealth(() => securityHeaders.GetRecommendedHeaders(false))
                },
                Timestamp = DateTime.UtcNow,
                ApiType = "Minimal API",
                Message = "Enterprise services health summary"
            };

            return Results.Ok(healthSummary);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error checking health: {ex.Message}");
        }
    }

    private static async Task<object> CheckServiceHealth<T>(Func<T> serviceCall)
    {
        try
        {
            await Task.Run(serviceCall);
            return new { Status = "Healthy", Message = "Service is operational" };
        }
        catch (Exception ex)
        {
            return new { Status = "Unhealthy", Message = ex.Message };
        }
    }

    private static IResult ValidateDataMinimal(
        TestDataModel model,
        ILogger<Program> logger)
    {
        // Manual validation for minimal API (since filters don't apply automatically)
        var validationResults = new List<string>();

        if (string.IsNullOrWhiteSpace(model.Name) || model.Name.Length < 3 || model.Name.Length > 100)
        {
            validationResults.Add("Name must be between 3 and 100 characters");
        }

        if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains('@'))
        {
            validationResults.Add("Valid email address is required");
        }

        if (model.Age < 18 || model.Age > 120)
        {
            validationResults.Add("Age must be between 18 and 120");
        }

        if (validationResults.Any())
        {
            logger.LogWarning("Validation failed for minimal API endpoint: {Errors}",
                string.Join(", ", validationResults));

            return Results.BadRequest(new
            {
                Message = "Validation failed",
                Errors = validationResults,
                ApiType = "Minimal API"
            });
        }

        logger.LogInformation("Data validation passed for minimal API endpoint");

        return Results.Ok(new
        {
            Message = "Data validation passed",
            ReceivedData = model,
            ApiType = "Minimal API"
        });
    }
}

/// <summary>
/// Test model for validation demonstration in minimal APIs.
/// </summary>
public class TestDataModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? PhoneNumber { get; set; }
}