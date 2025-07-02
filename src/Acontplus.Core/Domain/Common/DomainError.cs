using Acontplus.Core.Extensions;
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
    public static DomainError NotFound(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.NotFound, code, message, target, details);

    public static DomainError Validation(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Validation, code, message, target, details);

    public static DomainError Conflict(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Conflict, code, message, target, details);

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

    public static DomainError Internal(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.Internal, code, message, target, details);

    public static DomainError External(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.External, code, message, target, details);

    public static DomainError RateLimited(
        string code, string message,
        string? target = null,
        Dictionary<string, object>? details = null) =>
        new(ErrorType.RateLimited, code, message, target, details);

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

    public ApiError ToApiError() => new(
          Code: Code,
          Message: Message,
          Target: Target,
          Details: Details,
          Severity: Type.ToSeverityString(),
          Category: Type.ToCategoryString(),
          TraceId: Activity.Current?.Id,
          HelpUrl: $"https://errors.acontplus.com/{Code}"
      );

    public HttpStatusCode GetHttpStatusCode() => Type.ToHttpStatusCode();

    public ApiResponse<T> ToApiResponse<T>(string? correlationId = null) =>
        ApiResponse<T>.Failure(
            error: this.ToApiError(),
            message: Message,
            correlationId: correlationId,
            statusCode: Type.ToHttpStatusCode()
        );
}