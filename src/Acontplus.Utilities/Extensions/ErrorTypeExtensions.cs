using Acontplus.Core.Domain.Enums;
using System.Net;

namespace Acontplus.Utilities.Extensions;

public static class ErrorTypeExtensions
{
    public static HttpStatusCode ToHttpStatusCode(this ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound => HttpStatusCode.NotFound,
        ErrorType.Validation => HttpStatusCode.BadRequest,
        ErrorType.Conflict => HttpStatusCode.Conflict,
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
        ErrorType.Forbidden => HttpStatusCode.Forbidden,

        _ => HttpStatusCode.InternalServerError
    };
}
