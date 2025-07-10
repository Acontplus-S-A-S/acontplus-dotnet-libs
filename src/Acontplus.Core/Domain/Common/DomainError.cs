using System.Diagnostics;
using System.Net;

namespace Acontplus.Core.Domain.Common;

public readonly record struct DomainError(
    ErrorType Type,
    string Code,
    string Message,
    string? Target = null,
    Dictionary<string, object>? Details = null)
{
    #region Client Errors (4xx)

    public static DomainError BadRequest(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.BadRequest, code, message, target, details);

    public static DomainError Unauthorized(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Unauthorized, code, message, target, details);

    public static DomainError Forbidden(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Forbidden, code, message, target, details);

    public static DomainError NotFound(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.NotFound, code, message, target, details);

    public static DomainError MethodNotAllowed(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.MethodNotAllowed, code, message, target, details);

    public static DomainError NotAcceptable(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.NotAcceptable, code, message, target, details);

    public static DomainError Conflict(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Conflict, code, message, target, details);

    public static DomainError Validation(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Validation, code, message, target, details);

    public static DomainError PayloadTooLarge(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.PayloadTooLarge, code, message, target, details);

    public static DomainError UriTooLong(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.UriTooLong, code, message, target, details);

    public static DomainError UnsupportedMediaType(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.UnsupportedMediaType, code, message, target, details);

    public static DomainError RangeNotSatisfiable(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.RangeNotSatisfiable, code, message, target, details);

    public static DomainError ExpectationFailed(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.ExpectationFailed, code, message, target, details);

    public static DomainError PreconditionFailed(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.PreconditionFailed, code, message, target, details);

    public static DomainError PreconditionRequired(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.PreconditionRequired, code, message, target, details);

    public static DomainError RequestHeadersTooLarge(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.RequestHeadersTooLarge, code, message, target, details);

    public static DomainError UnavailableForLegal(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.UnavailableForLegal, code, message, target, details);

    public static DomainError RateLimited(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.RateLimited, code, message, target, details);

    #endregion

    #region Server Errors (5xx)

    public static DomainError Internal(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Internal, code, message, target, details);

    public static DomainError NotImplemented(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.NotImplemented, code, message, target, details);

    public static DomainError External(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.External, code, message, target, details);

    public static DomainError ServiceUnavailable(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.ServiceUnavailable, code, message, target, details);

    public static DomainError Timeout(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Timeout, code, message, target, details);

    public static DomainError RequestTimeout(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.RequestTimeout, code, message, target, details);

    public static DomainError HttpVersionNotSupported(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.HttpVersionNotSupported, code, message, target, details);

    public static DomainError InsufficientStorage(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.InsufficientStorage, code, message, target, details);

    public static DomainError LoopDetected(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.LoopDetected, code, message, target, details);

    public static DomainError NotExtended(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.NotExtended, code, message, target, details);

    public static DomainError NetworkAuthRequired(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.NetworkAuthRequired, code, message, target, details);

    #endregion

    public ApiError ToApiError() => new(
        Code: Code,
        Message: Message,
        Target: Target,
        Details: Details,
        Severity: Type.ToSeverityString(),
        Category: Type.ToCategoryString(),
        TraceId: Activity.Current?.Id,
        HelpUrl: GetHelpUrl()
    );

    public HttpStatusCode GetHttpStatusCode() => Type.ToHttpStatusCode();

    private string GetHelpUrl() => Type switch
    {
        ErrorType.Validation => $"https://errors.acontplus.com/validation/{Code.ToLowerInvariant()}",
        ErrorType.NotFound => $"https://errors.acontplus.com/not-found/{Code.ToLowerInvariant()}",
        ErrorType.Conflict => $"https://errors.acontplus.com/conflict/{Code.ToLowerInvariant()}",
        _ => $"https://errors.acontplus.com/{Type.ToString().ToLowerInvariant()}/{Code.ToLowerInvariant()}"
    };
}