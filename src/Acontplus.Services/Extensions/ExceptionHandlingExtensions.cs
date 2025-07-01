using Acontplus.Services.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Acontplus.Services.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseAcontplusExceptionHandling(
        this IApplicationBuilder app,
        Action<ExceptionHandlingOptions>? configure = null)
    {
        var options = new ExceptionHandlingOptions();
        configure?.Invoke(options);

        return app.UseMiddleware<ApiExceptionMiddleware>(options);
    }
}

public class ExceptionHandlingOptions
{
    /// <summary>
    /// Includes HTTP request details (Method, Path, etc.) in logs. Default: true.
    /// </summary>
    public bool IncludeRequestDetails { get; set; } = true;

    /// <summary>
    /// Logs the request body when an error occurs (caution: sensitive data). Default: false.
    /// </summary>
    public bool LogRequestBody { get; set; } = false;

    /// <summary>
    /// Includes stack traces and exception details in API responses (for debugging). Default: false.
    /// </summary>
    public bool IncludeDebugDetailsInResponse { get; set; } = false;
}