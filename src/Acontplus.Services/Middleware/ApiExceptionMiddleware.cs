using System.Text.Json;

namespace Acontplus.Services.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    private readonly bool _includeDebugDetails;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger,
        bool includeDebugDetails = false)
    {
        _next = next;
        _logger = logger;
        _includeDebugDetails = includeDebugDetails;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _includeDebugDetails
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (!context.Response.HasStarted && context.Response.StatusCode >= 400)
            {
                await HandleStatusCodeAsync(context);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        var correlationId = context.TraceIdentifier;
        LogException(ex, correlationId, context);

        var response = ex switch
        {
            ValidationException validationEx => HandleValidationException(validationEx, correlationId),
            ApiException apiEx => HandleApiException(apiEx, correlationId),
            _ => HandleUnhandledException(ex, correlationId)
        };

        context.Response.StatusCode = int.Parse(response.Code);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private ApiResponse HandleValidationException(ValidationException ex, string correlationId)
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
            statusCode: ex.StatusCode);
    }

    private ApiResponse HandleApiException(ApiException ex, string correlationId)
    {
        var error = new ApiError(
            Code: ex.ErrorCode,
            Message: ex.Message,
            Category: GetErrorCategory(ex.StatusCode));

        return ApiResponse.Failure(
            error,
            message: ex.Message,
            correlationId: correlationId,
            statusCode: ex.StatusCode);
    }

    private ApiResponse HandleUnhandledException(Exception ex, string correlationId)
    {
        var error = new ApiError(
            Code: "UNHANDLED_ERROR",
            Message: "An unexpected error occurred",
            Category: "system",
            Debug: _includeDebugDetails ? new
            {
                type = ex.GetType().Name,
                message = ex.Message,
                stackTrace = ex.StackTrace
            } : null);

        return ApiResponse.Failure(
            error,
            message: "An unexpected error occurred",
            correlationId: correlationId,
            statusCode: HttpStatusCode.InternalServerError);
    }

    private async Task HandleStatusCodeAsync(HttpContext context)
    {
        var statusCode = (HttpStatusCode)context.Response.StatusCode;
        var correlationId = context.TraceIdentifier;

        var error = new ApiError(
            Code: statusCode.ToString(),
            Message: GetStatusMessage(statusCode),
            Category: GetErrorCategory(statusCode));

        var response = ApiResponse.Failure(
            error,
            message: GetStatusMessage(statusCode),
            correlationId: correlationId,
            statusCode: statusCode);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private void LogException(Exception ex, string correlationId, HttpContext context)
    {
        _logger.LogError(ex, """
            CorrelationId: {CorrelationId}
            Path: {Path}
            Method: {Method}
            Exception: {ExceptionType}
            Message: {Message}
            """,
            correlationId,
            context.Request.Path,
            context.Request.Method,
            ex.GetType().Name,
            ex.Message);
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
}