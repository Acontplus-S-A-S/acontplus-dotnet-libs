namespace Acontplus.Core.Extensions;

/// <summary>
/// Extension methods for enhanced usability
/// </summary>
public static class ApiResponseExtensions
{
    /// <summary>
    /// Adds pagination metadata
    /// </summary>
    public static Dictionary<string, object> WithPagination(
        this Dictionary<string, object>? metadata,
        int page,
        int pageSize,
        int totalCount)
    {
        var result = metadata ?? new Dictionary<string, object>();

        result[ApiMetadataKeys.Pagination] = new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };

        return result;
    }

    /// <summary>
    /// Adds execution time metadata
    /// </summary>
    public static Dictionary<string, object> WithExecutionTime(
        this Dictionary<string, object>? metadata,
        TimeSpan executionTime)
    {
        var result = metadata ?? new Dictionary<string, object>();
        result[ApiMetadataKeys.ExecutionTime] = $"{executionTime.TotalMilliseconds}ms";
        return result;
    }
}