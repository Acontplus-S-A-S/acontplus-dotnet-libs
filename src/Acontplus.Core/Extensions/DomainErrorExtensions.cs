using System.Net;

namespace Acontplus.Core.Extensions;

public static class DomainErrorExtensions
{
    public static ApiResponse<T> ToApiResponse<T>(
        this DomainError error,
        string? correlationId = null)
    {
        return ApiResponse<T>.Failure(
            error: error.ToApiError(),
            message: error.Message,
            correlationId: correlationId,
            statusCode: error.Type.ToHttpStatusCode());
    }

    public static ApiResponse<T> ToApiResponse<T>(
        this IEnumerable<DomainError> errors,
        string? correlationId = null)
    {
        var errorList = errors.ToList();
        var primaryError = errorList.GetMostSevereError();

        return ApiResponse<T>.Failure(
            errors: errorList.ToApiErrors(),
            message: primaryError.Message,
            correlationId: correlationId,
            statusCode: primaryError.Type.ToHttpStatusCode());
    }

    public static IEnumerable<ApiError> ToApiErrors(
        this IEnumerable<DomainError> errors)
    {
        return errors.Select(e => e.ToApiError());
    }

    public static DomainError GetMostSevereError(
        this IEnumerable<DomainError> errors)
    {
        var severityOrder = new Dictionary<ErrorType, int>
        {
            [ErrorType.Validation] = 1,
            [ErrorType.NotFound] = 2,
            [ErrorType.Conflict] = 3,
            [ErrorType.Unauthorized] = 4,
            [ErrorType.Forbidden] = 5,
            [ErrorType.RateLimited] = 6,
            [ErrorType.External] = 7,
            [ErrorType.ServiceUnavailable] = 8,
            [ErrorType.Timeout] = 9,
            [ErrorType.Internal] = 10
        };

        return errors.MaxBy(e => severityOrder.GetValueOrDefault(e.Type, 0));
    }
}