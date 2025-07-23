namespace Acontplus.Core.Domain.Common.Results;

/// <summary>
/// Represents multiple domain errors that can occur during validation or complex operations.
/// </summary>
/// <param name="Errors">Collection of individual domain errors.</param>
public readonly record struct DomainErrors(IReadOnlyList<DomainError> Errors)
{
    private static readonly ErrorType[] SeverityOrder =
    {
        // Server Errors (5xx) - highest severity first
        ErrorType.Internal,
        ErrorType.External,
        ErrorType.ServiceUnavailable,
        ErrorType.Timeout,
        ErrorType.NotImplemented,
        ErrorType.HttpVersionNotSupported,
        ErrorType.InsufficientStorage,
        ErrorType.LoopDetected,
        ErrorType.NotExtended,
        ErrorType.NetworkAuthRequired,

        // Client Errors (4xx) - higher severity first
        ErrorType.RequestTimeout,
        ErrorType.UnavailableForLegal,
        ErrorType.Forbidden,
        ErrorType.Unauthorized,
        ErrorType.RateLimited,
        ErrorType.Conflict,
        ErrorType.NotFound,
        ErrorType.Validation,
        ErrorType.BadRequest,
        ErrorType.MethodNotAllowed,
        ErrorType.NotAcceptable,
        ErrorType.PayloadTooLarge,
        ErrorType.UriTooLong,
        ErrorType.UnsupportedMediaType,
        ErrorType.RangeNotSatisfiable,
        ErrorType.ExpectationFailed,
        ErrorType.PreconditionFailed,
        ErrorType.PreconditionRequired,
        ErrorType.RequestHeadersTooLarge
    };

    public static DomainErrors FromSingle(DomainError error) => new([error]);
    public static DomainErrors Multiple(params DomainError[] errors) => new(errors);
    public static DomainErrors Multiple(IEnumerable<DomainError> errors) => new(errors.ToList());

    public static implicit operator DomainErrors(DomainError error) => FromSingle(error);
    public static implicit operator DomainErrors(DomainError[] errors) => Multiple(errors);
    public static implicit operator DomainErrors(List<DomainError> errors) => Multiple(errors);

    public bool HasErrorsOfType(ErrorType type) => Errors.Any(e => e.Type == type);
    public IEnumerable<DomainError> GetErrorsOfType(ErrorType type) => Errors.Where(e => e.Type == type);

    public ErrorType GetMostSevereErrorType() => Errors
        .Select(e => e.Type)
        .OrderBy(t => Array.IndexOf(SeverityOrder, t))
        .FirstOrDefault();

    public string GetAggregateErrorMessage() => Errors.Count switch
    {
        0 => "No errors provided",
        1 => Errors[0].Message,
        _ => $"Multiple errors occurred ({Errors.Count}): " +
             string.Join("; ", Errors.Select(e => $"[{e.Type}] {e.Message}"))
    };
}
