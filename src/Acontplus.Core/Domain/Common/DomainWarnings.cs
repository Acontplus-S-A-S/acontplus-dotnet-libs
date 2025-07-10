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

    public bool HasWarnings => Warnings.Count > 0;
    public bool HasWarningsOfType(ErrorType type) => Warnings.Any(w => w.Type == type);
    public IEnumerable<DomainError> GetWarningsOfType(ErrorType type) => Warnings.Where(w => w.Type == type);

    public string GetAggregateWarningMessage() => Warnings.Count switch
    {
        0 => "No warnings",
        1 => Warnings[0].Message,
        _ => $"Multiple warnings occurred ({Warnings.Count}): " +
             string.Join("; ", Warnings.Select(w => $"[{w.Type}] {w.Message}"))
    };
}

/// <summary>
/// Represents a successful result that may also contain warnings.
/// </summary>
/// <param name="Value">The success value.</param>
/// <param name="Warnings">Optional warnings that occurred during the operation.</param>
public readonly record struct SuccessWithWarnings<TValue>(TValue Value, DomainWarnings? Warnings = null)
{
    public bool HasWarnings => Warnings?.HasWarnings ?? false;

    public static implicit operator SuccessWithWarnings<TValue>(TValue value) => new(value);

    public SuccessWithWarnings<TValue> AddWarning(DomainError warning) =>
        new(Value, Warnings?.Warnings.AddToCopy(warning) ?? DomainWarnings.Single(warning));

    public SuccessWithWarnings<TValue> AddWarnings(IEnumerable<DomainError> warnings) =>
        new(Value, Warnings?.Warnings.AddRangeToCopy(warnings) ?? DomainWarnings.Multiple(warnings));
}