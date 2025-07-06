using System.Collections.Immutable;

namespace Acontplus.Core.Extensions;

public static class DomainErrorExtensions
{
    private static readonly ImmutableDictionary<ErrorType, int> ErrorSeverity = new Dictionary<ErrorType, int>
    {
        [ErrorType.Internal] = 100,
        [ErrorType.External] = 90,
        [ErrorType.ServiceUnavailable] = 80,
        [ErrorType.Timeout] = 70,
        [ErrorType.Forbidden] = 60,
        [ErrorType.Unauthorized] = 50,
        [ErrorType.RateLimited] = 40,
        [ErrorType.Conflict] = 30,
        [ErrorType.NotFound] = 20,
        [ErrorType.Validation] = 10
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