using Acontplus.Core.Domain.Common.Results;
using System.Collections.Immutable;
using System.Net;

namespace Acontplus.Core.Domain.Extensions;

public static class DomainErrorExtensions
{
    private static readonly ImmutableDictionary<ErrorType, int> ErrorSeverity =
        new Dictionary<ErrorType, int>
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

    // ================ Single Error Methods ================
    public static ApiResponse<T> ToApiResponse<T>(this DomainError error, string? correlationId = null)
        => CreateApiResponse<T>(errors: new[] { error }, correlationId: correlationId);

    public static ApiResponse<T> ToApiResponse<T>(
        this DomainError error,
        string? correlationId = null,
        DomainWarnings? warnings = null)
        => CreateApiResponse<T>(
            errors: new[] { error },
            warnings: warnings?.Warnings,
            correlationId: correlationId);

    // ================ Collection Methods ================
    public static ApiResponse<T> ToApiResponse<T>(
        this DomainErrors errors,
        string? correlationId = null,
        DomainWarnings? warnings = null)
        => CreateApiResponse<T>(
            errors: errors.Errors,
            warnings: warnings?.Warnings,
            message: errors.GetAggregateErrorMessage(),
            correlationId: correlationId);

    public static ApiResponse<T> ToApiResponse<T>(
        this IEnumerable<DomainError> errors,
        string? correlationId = null,
        DomainWarnings? warnings = null)
        => CreateApiResponse<T>(
            errors: errors.ToList(),
            warnings: warnings?.Warnings,
            correlationId: correlationId);

    // ================ Result<T> Conversions ================
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, string? correlationId = null)
        => result.Match(
            success: data => CreateApiResponse(data: data, correlationId: correlationId),
            failure: error => error.ToApiResponse<T>(correlationId));

    public static ApiResponse<T> ToApiResponse<T>(
        this Result<T> result,
        string successMessage,
        string? correlationId = null)
        => result.Match(
            success: data => CreateApiResponse(
                data: data,
                message: successMessage,
                correlationId: correlationId),
            failure: error => error.ToApiResponse<T>(correlationId));

    // ================ SuccessWithWarnings ================
    public static ApiResponse<T> ToApiResponse<T>(
        this SuccessWithWarnings<T> result,
        string? correlationId = null,
        DomainWarnings? warnings = null)
        => CreateApiResponse(
            warnings: warnings?.Warnings,
            data: result.Value,
            correlationId: correlationId);

    // ================ Error Conversion Helpers ================
    public static IReadOnlyList<ApiError>? ToApiErrors(this IReadOnlyList<DomainError> errors)
        => errors?.Select(e => e.ToApiError()).ToList();

    public static IReadOnlyList<ApiError>? ToApiErrors(this DomainErrors errors)
        => errors.Errors.ToApiErrors();

    public static ApiError[] ToApiErrorArray(this DomainErrors errors)
        => errors.ToApiErrors()?.ToArray() ?? Array.Empty<ApiError>();

    // ================ Error Analysis Helpers ================
    public static DomainError GetMostSevereError(this IEnumerable<DomainError> errors)
    {
        if (!errors.Any())
            throw new ArgumentException("No errors provided", nameof(errors));

        return errors.MaxBy(e => ErrorSeverity.GetValueOrDefault(e.Type, 0));
    }

    public static ErrorType GetMostSevereErrorType(this DomainErrors errors)
        => errors.Errors.GetMostSevereError().Type;

    public static string GetAggregateErrorMessage(this DomainErrors errors)
        => string.Join("; ", errors.Errors.Select(e => e.Message));

    // ================ Error Details Formatting ================
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