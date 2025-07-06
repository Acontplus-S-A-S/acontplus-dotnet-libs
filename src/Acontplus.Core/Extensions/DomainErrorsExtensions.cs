namespace Acontplus.Core.Extensions;

public static class DomainErrorsExtensions
{
    public static IEnumerable<ApiError> ToApiErrors(this DomainErrors errors)
        => errors.Errors.Select(e => e.ToApiError());

    public static ApiResponse<T> ToApiResponse<T>(
        this DomainErrors errors,
        string? correlationId = null)
    {
        var mostSevereError = errors.GetMostSevereErrorType();
        return ApiResponse<T>.Failure(
            errors: errors.ToApiErrors(),
            message: errors.GetAggregateErrorMessage(),
            correlationId: correlationId,
            statusCode: mostSevereError.ToHttpStatusCode()
        );
    }

    public static ApiError[] ToApiErrorArray(this DomainErrors errors)
        => errors.ToApiErrors().ToArray();

    public static Dictionary<string, object>? ToErrorDetails(this DomainErrors errors)
    {
        if (errors.Errors.Count == 0)
            return null;

        return new Dictionary<string, object>
        {
            ["errors"] = errors.Errors
                .Select((e, i) => new
                {
                    Index = i,
                    e.Type,
                    e.Code,
                    e.Target,
                    Severity = e.Type.ToSeverityString()
                })
                .ToList()
        };
    }
}
