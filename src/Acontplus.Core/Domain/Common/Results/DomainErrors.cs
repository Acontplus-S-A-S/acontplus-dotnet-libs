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

    #region Factory Methods

    public static DomainErrors FromSingle(DomainError error) => new([error]);
    public static DomainErrors Multiple(params DomainError[] errors) => new(errors);
    public static DomainErrors Multiple(IEnumerable<DomainError> errors) => new(errors.ToList());

    #endregion

    #region Implicit Conversions

    public static implicit operator DomainErrors(DomainError error) => FromSingle(error);
    public static implicit operator DomainErrors(DomainError[] errors) => Multiple(errors);
    public static implicit operator DomainErrors(List<DomainError> errors) => Multiple(errors);

    #endregion

    #region Result Factory Methods

    /// <summary>
    /// Creates a failed Result&lt;TValue&gt; with these domain errors.
    /// Note: Only the first error will be used as Result&lt;TValue&gt; accepts single DomainError.
    /// Consider using Result&lt;TValue, DomainErrors&gt; for multiple errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <returns>A failed Result&lt;TValue&gt; containing the first error.</returns>
    public Result<TValue> Failure<TValue>() =>
        Errors.Count > 0
            ? Result<TValue>.Failure(Errors[0])
            : throw new InvalidOperationException("Cannot create failure result from empty error collection");

    /// <summary>
    /// Creates a failed Result&lt;TValue, DomainErrors&gt; with these domain errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <returns>A failed Result&lt;TValue, DomainErrors&gt; containing these errors.</returns>
    public Result<TValue, DomainErrors> FailureMultiple<TValue>() => Result<TValue, DomainErrors>.Failure(this);

    #endregion

    #region Error Analysis Methods

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

    #endregion
}
