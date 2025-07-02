using System.Net;

namespace Acontplus.Core.Domain.Enums;

public static class ErrorTypeExtensions
{
    public static HttpStatusCode ToHttpStatusCode(this ErrorType errorType) =>
        errorType switch
        {
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.Validation => HttpStatusCode.BadRequest,
            ErrorType.Conflict => HttpStatusCode.Conflict,
            ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
            ErrorType.Forbidden => HttpStatusCode.Forbidden,
            ErrorType.RateLimited => HttpStatusCode.TooManyRequests,
            ErrorType.External => HttpStatusCode.BadGateway,
            ErrorType.ServiceUnavailable => HttpStatusCode.ServiceUnavailable,
            ErrorType.Timeout => HttpStatusCode.RequestTimeout,
            _ => HttpStatusCode.InternalServerError
        };

    public static string ToSeverityString(this ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "warning",
            _ => "error"
        };

    public static string ToCategoryString(this ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "validation",
            ErrorType.NotFound => "business",
            ErrorType.Conflict => "business",
            ErrorType.Unauthorized => "security",
            ErrorType.Forbidden => "security",
            ErrorType.RateLimited => "performance",
            ErrorType.External => "integration",
            _ => "system"
        };
}