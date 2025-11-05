namespace Acontplus.Core.Constants;

/// <summary>
/// Metadata keys specifically for pagination-related information in PagedResult.
/// Use these constants to ensure consistency across all paged query implementations.
/// </summary>
public static class PaginationMetadataKeys
{
    // Query Information
    public const string HasFilters = "hasFilters";
    public const string HasSearch = "hasSearch";
    public const string SearchTerm = "searchTerm";

    // Sorting Information
    public const string SortBy = "sortBy";
    public const string SortDirection = "sortDirection";
    public const string DefaultSort = "defaultSort";

    // Query Source (⚠️ Use with caution - may expose implementation details)
    // Recommended: Only include in Development/Staging environments, not Production
    public const string QuerySource = "querySource";

    // Query Source Values (internal use - avoid exposing in production metadata)
    internal const string QuerySourceStoredProcedure = "storedProcedure";
    internal const string QuerySourceRawQuery = "rawQuery";
    internal const string QuerySourceView = "view";
    internal const string QuerySourceFunction = "function";

    // Performance Metrics
    public const string QueryDuration = "queryDurationMs";
    public const string CountDuration = "countDurationMs";
    public const string TotalDuration = "totalDurationMs";

    // Filter Details (optional, for debugging/auditing)
    public const string FilterCount = "filterCount";
    public const string AppliedFilters = "appliedFilters";

    // Data Quality
    public const string IsPartialResult = "isPartialResult";
    public const string ResultQuality = "resultQuality";

    // Cache Information (for future use)
    public const string FromCache = "fromCache";
    public const string CacheExpiry = "cacheExpiry";
}
