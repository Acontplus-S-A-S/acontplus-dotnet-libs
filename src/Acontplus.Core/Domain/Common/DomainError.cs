using Acontplus.Core.Domain.Enums;
using Acontplus.Core.DTOs.Responses;

namespace Acontplus.Core.Domain.Common;

// <summary>
/// Represents a standardized error for domain operations.
/// </summary>
/// <param name="Type">The type/category of the error.</param>
/// <param name="Code">A specific error code for programmatic handling.</param>
/// <param name="Message">Human-readable error message.</param>
/// <param name="Details">Additional error details or context.</param>
public readonly record struct DomainError(
    ErrorType Type,
    string Code,
    string Message,
    Dictionary<string, object>? Details = null)
{
    //[SuppressMessage("Design", "CA1000",
    //    Justification = "Factory methods improve DomainError discoverability")]
    public static DomainError NotFound(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.NotFound, code, message, details);

    public static DomainError Validation(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.Validation, code, message, details);

    public static DomainError Conflict(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.Conflict, code, message, details);

    public static DomainError Unauthorized(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.Unauthorized, code, message, details);

    public static DomainError Forbidden(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.Forbidden, code, message, details);

    public static DomainError Internal(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.Internal, code, message, details);

    public static DomainError External(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.External, code, message, details);

    public static DomainError RateLimited(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.RateLimited, code, message, details);
    public static DomainError ServiceUnavailable(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.ServiceUnavailable, code, message, details);
    public static DomainError Timeout(string code, string message, Dictionary<string, object>? details = null) =>
        new(ErrorType.Timeout, code, message, details);

    // Convert to ApiError for response

    public ApiError ToApiError() => new(
        Code: Code,
        Message: Message,
        Details: Details,
        HelpUrl: $"https://errors.acontplus.com/{Code}");
}