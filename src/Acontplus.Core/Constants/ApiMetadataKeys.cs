namespace Acontplus.Core.Constants;

public static class ApiMetadataKeys
{
    // Core Response Metadata
    public const string TraceId = "traceId";
    public const string RequestId = "requestId";
    public const string CorrelationId = "correlationId";
    public const string ClientId = "clientId";
    public const string Issuer = "issuer";
    public const string Version = "apiVersion";
    public const string Environment = "env";

    // Pagination (aligned with PagedResult properties)
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
