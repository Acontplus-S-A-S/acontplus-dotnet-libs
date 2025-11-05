namespace Acontplus.Core.Configuration;

/// <summary>
/// Configuration options for controlling pagination metadata exposure.
/// Allows fine-grained control over what metadata is returned in PagedResult.
/// </summary>
public class PaginationMetadataOptions
{
    /// <summary>
    /// Section name for configuration binding.
    /// </summary>
    public const string SectionName = "PaginationMetadata";

    /// <summary>
    /// Include query source information (e.g., stored procedure name, raw query indicator).
    /// ⚠️ Security: Set to false in production to avoid exposing implementation details.
    /// Default: false (secure by default)
    /// </summary>
    public bool IncludeQuerySource { get; set; } = false;

    /// <summary>
    /// Include stored procedure names in metadata.
    /// ⚠️ Security: Only enable in development/staging environments.
    /// Default: false (secure by default)
    /// </summary>
    public bool IncludeStoredProcedureName { get; set; } = false;

    /// <summary>
    /// Include performance metrics (query duration, count duration).
    /// Generally safe to expose, useful for monitoring.
    /// Default: true
    /// </summary>
    public bool IncludePerformanceMetrics { get; set; } = true;

    /// <summary>
    /// Include filter and search information.
    /// Generally safe, helps clients understand the results.
    /// Default: true
    /// </summary>
    public bool IncludeFilterInfo { get; set; } = true;

    /// <summary>
    /// Include sorting information.
    /// Generally safe, helps clients understand the results.
    /// Default: true
    /// </summary>
    public bool IncludeSortInfo { get; set; } = true;

    /// <summary>
    /// Include applied filters details (for debugging).
    /// ⚠️ Security: May expose sensitive filter values.
    /// Default: false (secure by default)
    /// </summary>
    public bool IncludeAppliedFilters { get; set; } = false;

    /// <summary>
    /// Validate configuration on startup.
    /// Logs warnings if insecure settings are detected in production.
    /// </summary>
    public void Validate(string environmentName)
    {
        var isProduction = environmentName?.Equals("Production", StringComparison.OrdinalIgnoreCase) ?? false;

        if (isProduction)
        {
            if (IncludeQuerySource)
            {
                throw new InvalidOperationException(
                    "Security Warning: IncludeQuerySource is enabled in Production. " +
                    "This exposes implementation details and should be disabled.");
            }

            if (IncludeStoredProcedureName)
            {
                throw new InvalidOperationException(
                    "Security Warning: IncludeStoredProcedureName is enabled in Production. " +
                    "This exposes database schema information and should be disabled.");
            }

            if (IncludeAppliedFilters)
            {
                throw new InvalidOperationException(
                    "Security Warning: IncludeAppliedFilters is enabled in Production. " +
                    "This may expose sensitive filter values and should be disabled.");
            }
        }
    }
}
