namespace Acontplus.Core.Domain.Common;

/// <summary>
/// Represents warnings that don't prevent operation success but should be communicated.
/// </summary>
/// <param name="Warnings">Collection of individual warnings.</param>
public readonly record struct DomainWarnings(IReadOnlyList<DomainError> Warnings)
{
    public static DomainWarnings Single(DomainError warning) => new([warning]);
    public static DomainWarnings Multiple(params DomainError[] warnings) => new(warnings);
    public static DomainWarnings Multiple(IEnumerable<DomainError> warnings) => new(warnings.ToList());

    public static implicit operator DomainWarnings(DomainError warning) => Single(warning);
    public static implicit operator DomainWarnings(DomainError[] warnings) => Multiple(warnings);
    public static implicit operator DomainWarnings(List<DomainError> warnings) => Multiple(warnings);

    // Convert to ApiError collection for response
    public IEnumerable<ApiError> ToApiErrors() => Warnings.Select(w => w.ToApiError());
}

/// <summary>
/// Represents a successful result that may also contain warnings.
/// </summary>
/// <param name="Value">The success value.</param>
/// <param name="Warnings">Optional warnings that occurred during the operation.</param>
public readonly record struct SuccessWithWarnings<TValue>(TValue Value, DomainWarnings? Warnings = null)
{
    public bool HasWarnings => Warnings?.Warnings.Count > 0;
    public static implicit operator SuccessWithWarnings<TValue>(TValue value) => new(value);
}