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
    public bool IncludeRequestDetails { get; set; } = true;
    public bool LogRequestBody { get; set; } = true;
}