using System.Collections.Immutable;

namespace Acontplus.Core.Extensions;

public static class DomainErrorExtensions
{
    private static readonly ImmutableDictionary<ErrorType, int> ErrorSeverity = new Dictionary<ErrorType, int>
    {
        // Server Errors (5xx) - highest severity
        [ErrorType.Internal] = 100,
        [ErrorType.External] = 95,
        [ErrorType.ServiceUnavailable] = 90,
        [ErrorType.Timeout] = 85,
        [ErrorType.NotImplemented] = 80,
        [ErrorType.HttpVersionNotSupported] = 75,
        [ErrorType.InsufficientStorage] = 70,
        [ErrorType.LoopDetected] = 65,
        [ErrorType.NotExtended] = 60,
        [ErrorType.NetworkAuthRequired] = 55,

        // Client Errors (4xx) - lower severity
        [ErrorType.RequestTimeout] = 50,
        [ErrorType.UnavailableForLegal] = 45,
        [ErrorType.Forbidden] = 40,
        [ErrorType.Unauthorized] = 35,
        [ErrorType.RateLimited] = 30,
        [ErrorType.Conflict] = 25,
        [ErrorType.NotFound] = 20,
        [ErrorType.Validation] = 15,
        [ErrorType.BadRequest] = 10,
        [ErrorType.MethodNotAllowed] = 9,
        [ErrorType.NotAcceptable] = 8,
        [ErrorType.PayloadTooLarge] = 7,
        [ErrorType.UriTooLong] = 6,
        [ErrorType.UnsupportedMediaType] = 5,
        [ErrorType.RangeNotSatisfiable] = 4,
        [ErrorType.ExpectationFailed] = 3,
        [ErrorType.PreconditionFailed] = 2,
        [ErrorType.PreconditionRequired] = 1,
        [ErrorType.RequestHeadersTooLarge] = 0
    }.ToImmutableDictionary();

    public static ApiResponse<T> ToApiResponse<T>(
        this DomainError error,
        string? correlationId = null,
        DomainWarnings? warnings = null)
    {
        return ApiResponse<T>.Failure(
            error: error.ToApiError(),
            message: error.Message,
            correlationId: correlationId,
            statusCode: error.Type.ToHttpStatusCode(),
            warnings: warnings?.ToApiErrors());
    }

    public static ApiResponse<T> ToApiResponse<T>(
        this DomainErrors errors,
        string? correlationId = null,
        DomainWarnings? warnings = null)
    {
        var primaryError = errors.GetMostSevereErrorType();
        return ApiResponse<T>.Failure(
            errors: errors.ToApiErrors(),
            message: errors.GetAggregateErrorMessage(),
            correlationId: correlationId,
            statusCode: primaryError.ToHttpStatusCode(),
            warnings: warnings?.ToApiErrors());
    }

    public static ApiResponse<T> ToApiResponse<T>(
        this IEnumerable<DomainError> errors,
        string? correlationId = null,
        DomainWarnings? warnings = null)
    {
        var errorList = errors.ToList();
        var primaryError = errorList.GetMostSevereError();
        return ApiResponse<T>.Failure(
            errors: errorList.ToApiErrors(),
            message: primaryError.Message,
            correlationId: correlationId,
            statusCode: primaryError.Type.ToHttpStatusCode(),
            warnings: warnings?.ToApiErrors());
    }

    public static IEnumerable<ApiError> ToApiErrors(this IEnumerable<DomainError> errors)
        => errors.Select(e => e.ToApiError());

    public static DomainError GetMostSevereError(this IEnumerable<DomainError> errors)
    {
        if (!errors.Any())
        {
            throw new ArgumentException("No errors provided", nameof(errors));
        }

        return errors.MaxBy(e => ErrorSeverity.GetValueOrDefault(e.Type, 0));
    }
}