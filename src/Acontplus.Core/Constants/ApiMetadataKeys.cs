namespace Acontplus.Core.Constants;

/// <summary>
/// Metadata keys for API-level response metadata (tracing, correlation, versioning, etc.).
/// For pagination-specific metadata, see <see cref="PaginationMetadataKeys"/>.
/// </summary>
public static class ApiMetadataKeys
{
    // Core Response Metadata
    public const string TraceId = "traceId";
    public const string RequestId = "requestId";
    public const string CorrelationId = "correlationId";
    public const string ClientId = "clientId";
    public const string Issuer = "issuer";
    public const string TenantId = "tenantId";
    public const string TimestampUtc = "timestampUtc";
    public const string Version = "apiVersion";
    public const string Environment = "env";

    // Pagination Container (wraps PagedResult data for API responses)
    // Note: Individual pagination fields (pageIndex, pageSize, etc.) are part of PagedResult itself
    // This key is used when you need to nest pagination info in API metadata
    public const string Pagination = "paging";
    public const string PageIndex = "pageIndex";
    public const string PageSize = "pageSize";
    public const string TotalCount = "totalCount";
    public const string TotalPages = "totalPages";
    public const string HasNextPage = "hasNextPage";
    public const string HasPreviousPage = "hasPreviousPage";
    public const string Links = "links";  // For HATEOAS when needed

    // Performance
    public const string Duration = "durationMs";

    // Optional Extended Metadata
    public const string Deprecation = "deprecation";
    public const string RateLimit = "rateLimit";
}
