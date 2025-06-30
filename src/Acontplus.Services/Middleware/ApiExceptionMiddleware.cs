using System.Text.Json;

namespace Acontplus.Services.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment()
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (!context.Response.HasStarted &&
                context.Response.StatusCode >= 400)
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
        var correlationId = context.TraceIdentifier;
        LogException(ex, correlationId, context);

        var response = ex switch
        {
            ApiException apiEx => CreateApiErrorResponse(apiEx, correlationId),
            _ => CreateUnhandledErrorResponse(ex, correlationId)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private async Task HandleStatusCodeAsync(HttpContext context)
    {
        var statusCode = (HttpStatusCode)context.Response.StatusCode;
        var correlationId = context.TraceIdentifier;

        var response = new ApiResponse
        {
            Status = "error",
            Code = ((int)statusCode).ToString(),
            Message = GetStatusMessage(statusCode),
            CorrelationId = correlationId,
            Errors = [new ApiError(statusCode.ToString(), GetStatusMessage(statusCode))]
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private ApiResponse CreateApiErrorResponse(ApiException ex, string correlationId)
    {
        var response = new ApiResponse
        {
            Status = "error",
            Code = ex.ErrorCode,
            Message = ex.Message,
            CorrelationId = correlationId,
            Errors = [CreateApiError(ex)]
        };

        if (ex is ValidationException validationEx)
        {
            response.Errors = validationEx.Errors
                .SelectMany(e => e.Value.Select(m =>
                    new ApiError("VALIDATION_ERROR", m, e.Key)))
                .ToList();
        }

        if (_env.IsDevelopment())
        {
            response.Metadata = new Dictionary<string, object>
            {
                ["stackTrace"] = ex.StackTrace?.Replace(Environment.NewLine, "\n") ?? string.Empty,
                ["exceptionType"] = ex.GetType().Name
            };
        }

        return response;
    }

    private ApiResponse CreateUnhandledErrorResponse(Exception ex, string correlationId)
    {
        return new ApiResponse
        {
            Status = "error",
            Code = "INTERNAL_ERROR",
            Message = "An unexpected error occurred",
            CorrelationId = correlationId,
            Errors = [new ApiError("SYSTEM_FAILURE", "System failure")],
            Metadata = _env.IsDevelopment() ? new Dictionary<string, object>
            {
                ["stackTrace"] = ex.StackTrace?.Replace(Environment.NewLine, "\n") ?? string.Empty,
                ["exceptionType"] = ex.GetType().Name,
                ["detail"] = ex.Message
            } : null
        };
    }

    private ApiError CreateApiError(ApiException ex)
    {
        return new ApiError(
            Code: ex.ErrorCode,
            Message: ex.Message,
            Category: ex switch
            {
                ValidationException => "validation",
                NotFoundException => "not_found",
                UnauthorizedException => "authentication",
                ConflictException => "conflict",
                _ => "system"
            });
    }

    private void LogException(Exception ex, string correlationId, HttpContext context)
    {
        var logMessage = new StringBuilder()
            .AppendLine($"CorrelationId: {correlationId}")
            .AppendLine($"Path: {context.Request.Path}")
            .AppendLine($"Method: {context.Request.Method}")
            .AppendLine($"Exception: {ex.GetType().Name}")
            .AppendLine($"Message: {ex.Message}");

        if (context.Request.Method == HttpMethods.Post ||
            context.Request.Method == HttpMethods.Put)
        {
            logMessage.AppendLine($"Body: {GetRequestBody(context)}");
        }

        _logger.LogError("{LogMessage}", logMessage.ToString());
    }

    private string GetRequestBody(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = reader.ReadToEndAsync().GetAwaiter().GetResult();
            context.Request.Body.Position = 0;
            return body;
        }
        catch
        {
            return "Unable to read request body";
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
}