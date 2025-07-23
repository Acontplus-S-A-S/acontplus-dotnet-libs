using Acontplus.Core.Domain.Common.Results;

namespace Acontplus.Core.Domain.Extensions;

public static class DomainWarningsExtensions
{
    public static IReadOnlyList<ApiError>? ToApiErrors(this DomainWarnings warnings)
        => (IReadOnlyList<ApiError>?)warnings.Warnings.Select(w => w.ToApiError());

    public static ApiError[] ToApiErrorArray(this DomainWarnings warnings)
        => warnings.ToApiErrors().ToArray();

    public static Dictionary<string, object>? ToWarningDetails(this DomainWarnings warnings)
    {
        if (!warnings.HasWarnings)
            return null;

        return new Dictionary<string, object>
        {
            ["warnings"] = warnings.Warnings
                .Select((w, i) => new
                {
                    Index = i,
                    w.Type,
                    w.Code,
                    w.Target,
                    Severity = w.Type.ToSeverityString()
                })
                .ToList()
        };
    }

    public static DomainWarnings AddToCopy(
        this IReadOnlyList<DomainError> warnings,
        DomainError warning)
    {
        var newWarnings = new List<DomainError>(warnings) { warning };
        return new DomainWarnings(newWarnings);
    }

    public static DomainWarnings AddRangeToCopy(
        this IReadOnlyList<DomainError> warnings,
        IEnumerable<DomainError> additionalWarnings)
    {
        var newWarnings = new List<DomainError>(warnings);
        newWarnings.AddRange(additionalWarnings);
        return new DomainWarnings(newWarnings);
    }
}

public static class SuccessWithWarningsExtensions
{
    public static SuccessWithWarnings<T> WithWarning<T>(
        this T value,
        DomainError warning) => new(value, DomainWarnings.FromSingle(warning));

    public static SuccessWithWarnings<T> WithWarnings<T>(
        this T value,
        IEnumerable<DomainError> warnings) => new(value, DomainWarnings.Multiple(warnings));
}