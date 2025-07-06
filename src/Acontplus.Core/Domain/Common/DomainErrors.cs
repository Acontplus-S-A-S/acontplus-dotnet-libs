namespace Acontplus.Core.Domain.Common;

/// <summary>
/// Represents multiple domain errors that can occur during validation or complex operations.
/// </summary>
/// <param name="Errors">Collection of individual domain errors.</param>
public readonly record struct DomainErrors(IReadOnlyList<DomainError> Errors)
{
    private static readonly ErrorType[] SeverityOrder =
    {
        ErrorType.Internal,
        ErrorType.External,
        ErrorType.ServiceUnavailable,
        ErrorType.Forbidden,
        ErrorType.Unauthorized,
        ErrorType.RateLimited,
        ErrorType.Conflict,
        ErrorType.NotFound,
        ErrorType.Validation
    };

    public static DomainErrors Single(DomainError error) => new([error]);
    public static DomainErrors Multiple(params DomainError[] errors) => new(errors);
    public static DomainErrors Multiple(IEnumerable<DomainError> errors) => new(errors.ToList());

    public static implicit operator DomainErrors(DomainError error) => Single(error);
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