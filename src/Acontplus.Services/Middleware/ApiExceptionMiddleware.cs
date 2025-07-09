using Acontplus.Services.Extensions;
using System.Diagnostics;
using System.Text.Json;

namespace Acontplus.Services.Middleware;

public class ApiExceptionMiddleware(
    RequestDelegate next,
    ILogger<ApiExceptionMiddleware> logger,
    ExceptionHandlingOptions options)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = options.IncludeDebugDetailsInResponse
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        var tenantId = GetTenantId(context);

        context.Items["CorrelationId"] = correlationId;
        context.Items["TenantId"] = tenantId;

        try
        {
            await next(context);

            if (!context.Response.HasStarted && context.Response.StatusCode >= 400)
            {
                await HandleStatusCodeAsync(context, correlationId, tenantId);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, correlationId, tenantId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, string correlationId, string tenantId)
    {
        context.Response.ContentType = "application/json";

        await LogException(ex, correlationId, tenantId, context);

        var response = ex switch
        {
            ValidationException validationEx => HandleValidationException(validationEx, correlationId, tenantId),
            ApiException apiEx => HandleApiException(apiEx, correlationId, tenantId),
            _ => HandleUnhandledException(ex, correlationId, tenantId)
        };

        context.Response.StatusCode = int.Parse(response.Code);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private static ApiResponse HandleValidationException(ValidationException ex, string correlationId, string tenantId)
    {
        var errors = ex.Errors
            .SelectMany(e => e.Value.Select(message =>
                new ApiError(
                    Code: "VALIDATION_ERROR",
                    Message: message,
                    Target: e.Key,
                    Category: "validation")))
            .ToList();

        return ApiResponse.Failure(
            errors,
            message: ex.Message,
            correlationId: correlationId,
            statusCode: ex.StatusCode,
            metadata: CreateStandardMetadata(tenantId));
    }

    private static ApiResponse HandleApiException(ApiException ex, string correlationId, string tenantId)
    {
        var error = new ApiError(
            Code: ex.ErrorCode,
            Message: ex.Message,
            Category: GetErrorCategory(ex.StatusCode));

        return ApiResponse.Failure(
            error,
            message: ex.Message,
            correlationId: correlationId,
            statusCode: ex.StatusCode,
            metadata: CreateStandardMetadata(tenantId));
    }

    private ApiResponse HandleUnhandledException(Exception ex, string correlationId, string tenantId)
    {
        var debugInfo = options.IncludeDebugDetailsInResponse
            ? GetSafeDebugInfo(ex)
            : null;

        var error = new ApiError(
            Code: "UNHANDLED_ERROR",
            Message: "An unexpected error occurred",
            Category: "system",
            Details: debugInfo != null
                ? new Dictionary<string, object>
                {
                    ["debug"] = debugInfo
                }
                : null,
            TraceId: Activity.Current?.TraceId.ToString());

        return ApiResponse.Failure(
            error,
            message: "An unexpected error occurred",
            correlationId: correlationId,
            statusCode: HttpStatusCode.InternalServerError,
            metadata: CreateStandardMetadata(tenantId));
    }

    private async Task HandleStatusCodeAsync(HttpContext context, string correlationId, string tenantId)
    {
        var statusCode = (HttpStatusCode)context.Response.StatusCode;

        var error = new ApiError(
            Code: statusCode.ToString(),
            Message: GetStatusMessage(statusCode),
            Category: GetErrorCategory(statusCode));

        var response = ApiResponse.Failure(
            error,
            message: GetStatusMessage(statusCode),
            correlationId: correlationId,
            statusCode: statusCode,
            metadata: CreateStandardMetadata(tenantId));

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private async Task LogException(Exception ex, string correlationId, string tenantId, HttpContext context)
    {
        var logMessage = new StringBuilder()
            .AppendLine($"CorrelationId: {correlationId}")
            .AppendLine($"TenantId: {tenantId}");

        if (options.IncludeRequestDetails)
        {
            logMessage.AppendLine($"Path: {context.Request.Path}")
                .AppendLine($"Method: {context.Request.Method}");
        }

        if (options.LogRequestBody && context.Request.Body.CanRead)
        {
            logMessage.AppendLine($"Request Body: {await ReadRequestBodyAsync(context.Request)}");
        }

        logger.LogError(ex, logMessage.ToString());
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        try
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
        catch
        {
            return "<Unable to read body>";
        }
    }

    private static string GetStatusMessage(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "Invalid request",
        HttpStatusCode.Unauthorized => "Authentication required",
        HttpStatusCode.Forbidden => "Access denied",
        HttpStatusCode.NotFound => "Resource not found",
        HttpStatusCode.Conflict => "Conflict occurred",
        HttpStatusCode.InternalServerError => "Internal server error",
        _ => statusCode.ToString()
    };

    private static string GetErrorCategory(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "validation",
        HttpStatusCode.Unauthorized => "authentication",
        HttpStatusCode.Forbidden => "authorization",
        HttpStatusCode.NotFound => "not_found",
        HttpStatusCode.Conflict => "conflict",
        _ => "system"
    };

    private static Dictionary<string, object>? GetSafeDebugInfo(Exception ex)
    {
        if (!ShouldIncludeDebugInfo())
            return null;

        return new Dictionary<string, object>
        {
            ["type"] = ex.GetType().Name,
            ["message"] = ex.Message,
            ["stackTrace"] = ex.StackTrace?.Split(Environment.NewLine) ?? Array.Empty<string>(),
            ["innerException"] = ex.InnerException != null
                ? new
                {
                    type = ex.InnerException.GetType().Name,
                    message = ex.InnerException.Message
                }
                : null,
            ["activityId"] = Activity.Current?.Id ?? "none"
        };
    }

    private static bool ShouldIncludeDebugInfo()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ||
               Debugger.IsAttached;
    }

    private static string GetCorrelationId(HttpContext context)
    {
        return context.Request.Headers.TryGetValue("Correlation-Id", out var headerValue) &&
               Guid.TryParse(headerValue, out var parsed)
            ? parsed.ToString()
            : context.TraceIdentifier;
    }

    private static string GetTenantId(HttpContext context)
    {
        return context.Request.Headers.TryGetValue("Tenant-Id", out var headerValue)
            ? headerValue.ToString()
            : "UNKNOWN";
    }

    private static Dictionary<string, object> CreateStandardMetadata(string tenantId)
    {
        return new Dictionary<string, object>
        {
            ["tenantId"] = tenantId,
            ["timestampUtc"] = DateTime.UtcNow
        };
    }
}