using Acontplus.Core.Domain.Enums;
using Acontplus.Core.DTOs.Responses;

namespace Acontplus.Core.Domain.Common;

/// <summary>
/// Represents multiple domain errors that can occur during validation or complex operations.
/// </summary>
/// <param name="Errors">Collection of individual domain errors.</param>
public readonly record struct DomainErrors(IReadOnlyList<DomainError> Errors)
{
    public static DomainErrors Single(DomainError error) => new([error]);
    public static DomainErrors Multiple(params DomainError[] errors) => new(errors);
    public static DomainErrors Multiple(IEnumerable<DomainError> errors) => new(errors.ToList());

    public static implicit operator DomainErrors(DomainError error) => Single(error);
    public static implicit operator DomainErrors(DomainError[] errors) => Multiple(errors);
    public static implicit operator DomainErrors(List<DomainError> errors) => Multiple(errors);

    public bool HasErrorsOfType(ErrorType type) => Errors.Any(e => e.Type == type);
    public IEnumerable<DomainError> GetErrorsOfType(ErrorType type) => Errors.Where(e => e.Type == type);

    // Convert to ApiError collection for response
    public IEnumerable<ApiError> ToApiErrors() => Errors.Select(e => e.ToApiError());

    public ApiError ToPrimaryApiError() => Errors.Count switch
    {
        0 => throw new InvalidOperationException("No errors present"),
        1 => Errors[0].ToApiError(),
        _ => new ApiError(
            Code: "MULTIPLE_ERRORS",
            Message: GetAggregateErrorMessage(this),
            Details: new Dictionary<string, object>
            {
                ["errorCount"] = Errors.Count,
                ["errorTypes"] = Errors.Select(e => e.Type.ToString()).Distinct()
            })
    };

    public ErrorType GetMostSevereErrorType()
    {
        var severityOrder = new[] {
        ErrorType.Internal,
        ErrorType.External,
        ErrorType.Forbidden,
        ErrorType.Unauthorized,
        ErrorType.RateLimited,
        ErrorType.Conflict,
        ErrorType.NotFound,
        ErrorType.Validation
    };

        return Errors
            .Select(e => e.Type)
            .OrderBy(t => Array.IndexOf(severityOrder, t))
            .First();
    }

    private static string GetAggregateErrorMessage(DomainErrors errors)
    {
        if (errors.Errors.Count == 1)
            return errors.Errors[0].Message;

        var errorTypes = errors.Errors
            .Select(e => e.Type)
            .Distinct();

        return $"Multiple errors occurred: {string.Join(", ", errorTypes)}";
    }
}
