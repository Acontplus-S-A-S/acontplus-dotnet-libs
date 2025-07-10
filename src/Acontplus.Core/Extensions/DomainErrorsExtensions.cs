namespace Acontplus.Core.Extensions;

public static class DomainErrorsExtensions
{
    public static IReadOnlyList<ApiError>? ToApiErrors(this DomainErrors errors)
        => errors.Errors.Select(e => e.ToApiError()) as IReadOnlyList<ApiError>;

    public static ApiResponse<T> ToApiResponse<T>(
        this DomainErrors errors,
        string? correlationId = null)
    {
        var mostSevereError = errors.GetMostSevereErrorType();
        var options = new ApiResponseOptions
        {
            Message = errors.GetAggregateErrorMessage(),
            CorrelationId = correlationId,
            StatusCode = mostSevereError.ToHttpStatusCode()
        };

        return ApiResponse<T>.Failure(errors.ToApiErrors(), options);
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