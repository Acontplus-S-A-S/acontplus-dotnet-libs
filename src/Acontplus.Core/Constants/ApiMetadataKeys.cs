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

    // Pagination (structured for client convenience)
    public const string Pagination = "paging";
    public const string Page = "page";
    public const string PageSize = "size";  // Shorter alternative to 'pageSize'
    public const string TotalItems = "total";
    public const string TotalPages = "pages";
    public const string HasNext = "hasNext";
    public const string HasPrev = "hasPrev";
    public const string Links = "links";  // For HATEOAS when needed

    // Performance
    public const string Duration = "durationMs";

    // Optional Extended Metadata
    public const string Deprecation = "deprecation";
    public const string RateLimit = "rateLimit";
}