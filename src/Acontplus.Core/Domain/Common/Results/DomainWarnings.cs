namespace Acontplus.Core.Domain.Common.Results;

/// <summary>
/// Represents warnings that don't prevent operation success but should be communicated.
/// </summary>
/// <param name="Warnings">Collection of individual warnings.</param>
public readonly record struct DomainWarnings(IReadOnlyList<DomainError> Warnings)
{
    public static DomainWarnings FromSingle(DomainError warning) => new([warning]);
    public static DomainWarnings Multiple(params DomainError[] warnings) => new(warnings);
    public static DomainWarnings Multiple(IEnumerable<DomainError> warnings) => new(warnings.ToList());

    public static implicit operator DomainWarnings(DomainError warning) => FromSingle(warning);
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
