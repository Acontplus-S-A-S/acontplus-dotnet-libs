using System.Net;

namespace Acontplus.Core.Extensions;

public static class DomainErrorsExtensions
{
    private static ApiResponse<T> CreateApiResponse<T>(
        IReadOnlyList<DomainError>? errors = null,
        IReadOnlyList<DomainError>? warnings = null,
        T? data = default,
        string? message = null,
        string? correlationId = null)
    {
        var options = new ApiResponseOptions
        {
            Message = message ?? errors?.FirstOrDefault().Message,
            Errors = errors?.ToApiErrors(),
            Warnings = warnings?.ToApiErrors(),
            CorrelationId = correlationId,
            StatusCode = errors?.GetMostSevereError().GetHttpStatusCode() ?? HttpStatusCode.OK
        };

        return errors == null || !errors.Any()
            ? ApiResponse<T>.Success(data!, options)
            : ApiResponse<T>.Failure(errors.ToApiErrors(), options);
    }

    // Your existing methods with improved consistency
    public static IReadOnlyList<ApiError>? ToApiErrors(this DomainErrors errors)
        => errors.Errors.Select(e => e.ToApiError()).ToList();

    public static ApiResponse<T> ToApiResponse<T>(
        this DomainErrors errors,
        string? correlationId = null,
        DomainWarnings? warnings = null)
    {
        return CreateApiResponse<T>(
            errors: errors.Errors,
            warnings: warnings?.Warnings,
            message: errors.GetAggregateErrorMessage(),
            correlationId: correlationId
        );
    }

    public static ApiError[] ToApiErrorArray(this DomainErrors errors)
        => errors.ToApiErrors()?.ToArray() ?? Array.Empty<ApiError>();

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

    // Additional helper methods
    public static ErrorType GetMostSevereErrorType(this DomainErrors errors)
        => errors.Errors.GetMostSevereError().Type;

    public static string GetAggregateErrorMessage(this DomainErrors errors)
        => string.Join("; ", errors.Errors.Select(e => e.Message));
}