using Acontplus.Core.Helpers;
using System.Net;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs;

public sealed class ApiResponse<T>
{
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("code")]
    public required string Code { get; init; }  // Now always uses HTTP status codes

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("errors")]
    public IEnumerable<ApiError>? Errors { get; init; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; init; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; } = DateTimeOffset.UtcNow.ToString("O");

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }

    // Success factory methods
    public static ApiResponse<T> Success(
        T data,
        string? message = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse<T>
        {
            Status = "success",
            Code = HttpStatusCodes.FromEnum(statusCode),
            Message = message ?? GetDefaultSuccessMessage(statusCode),
            Data = data,
            Metadata = metadata,
            CorrelationId = correlationId
        };
    }

    // Failure factory methods
    public static ApiResponse<T> Failure(
        IEnumerable<ApiError> errors,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T>
        {
            Status = "error",
            Code = HttpStatusCodes.FromEnum(statusCode),
            Message = message ?? GetDefaultErrorMessage(statusCode),
            Errors = errors,
            CorrelationId = correlationId
        };
    }

    public static ApiResponse<T> Failure(
        ApiError error,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return Failure([error], message, correlationId, statusCode);
    }

    // Warning factory method
    public static ApiResponse<T> Warning(
        T data,
        IEnumerable<ApiError> warnings,
        string? message = "Operation completed with warnings",
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse<T>
        {
            Status = "warning",
            Code = HttpStatusCodes.FromEnum(statusCode),
            Message = message,
            Data = data,
            Errors = warnings,
            CorrelationId = correlationId
        };
    }

    private static string GetDefaultSuccessMessage(HttpStatusCode statusCode)
    => ApiResponseHelpers.GetDefaultSuccessMessage(statusCode);

    private static string GetDefaultErrorMessage(HttpStatusCode statusCode)
        => ApiResponseHelpers.GetDefaultErrorMessage(statusCode);

    [JsonIgnore]
    public bool IsSuccess => Status == "success";

    [JsonIgnore]
    public bool IsError => Status == "error";

    [JsonIgnore]
    public bool HasWarnings => Status == "warning";

    [JsonIgnore]
    public bool HasData => Data is not null;

    [JsonIgnore]
    public bool HasErrors => Errors?.Any() == true;
}

public sealed class ApiResponse
{
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("errors")]
    public IEnumerable<ApiError>? Errors { get; init; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; init; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; } = DateTimeOffset.UtcNow.ToString("O");

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }

    public static ApiResponse Success(
        string? message = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse
        {
            Status = "success",
            Code = HttpStatusCodes.FromEnum(statusCode),
            Message = message ?? ApiResponseHelpers.GetDefaultSuccessMessage(statusCode),
            Metadata = metadata,
            CorrelationId = correlationId
        };
    }

    public static ApiResponse Failure(
        IEnumerable<ApiError> errors,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse
        {
            Status = "error",
            Code = HttpStatusCodes.FromEnum(statusCode),
            Message = message ?? ApiResponseHelpers.GetDefaultErrorMessage(statusCode),
            Errors = errors,
            CorrelationId = correlationId
        };
    }

    public static ApiResponse Failure(
        ApiError error,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return Failure([error], message, correlationId, statusCode);
    }

    public static ApiResponse Warning(
        IEnumerable<ApiError> warnings,
        string? message = "Operation completed with warnings",
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse
        {
            Status = "warning",
            Code = HttpStatusCodes.FromEnum(statusCode),
            Message = message,
            Errors = warnings,
            CorrelationId = correlationId
        };
    }

    [JsonIgnore]
    public bool IsSuccess => Status == "success";

    [JsonIgnore]
    public bool IsError => Status == "error";

    [JsonIgnore]
    public bool HasWarnings => Status == "warning";

    [JsonIgnore]
    public bool HasErrors => Errors?.Any() == true;
}

/// Enhanced error format with additional context for modern frontends
/// </summary>
public sealed record ApiError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("target")] string? Target = null,
    [property: JsonPropertyName("details")] Dictionary<string, string>? Details = null,
    [property: JsonPropertyName("severity")] string Severity = "error") // error, warning, info
{
    /// <summary>
    /// Additional context for debugging (only in development)
    /// </summary>
    [JsonPropertyName("debug")]
    public object? Debug { get; init; }

    /// <summary>
    /// Error category for frontend handling
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; init; } // validation, authorization, business, system

    /// <summary>
    /// Suggested action for the user
    /// </summary>
    [JsonPropertyName("suggestedAction")]
    public string? SuggestedAction { get; init; }
}

/// <summary>
/// Common metadata keys for consistent usage
/// </summary>
public static class ApiMetadataKeys
{
    public const string Pagination = "pagination";
    public const string TotalCount = "totalCount";
    public const string Page = "page";
    public const string PageSize = "pageSize";
    public const string HasNextPage = "hasNextPage";
    public const string HasPreviousPage = "hasPreviousPage";
    public const string ExecutionTime = "executionTime";
    public const string Version = "version";
    public const string Environment = "environment";
    public const string RequestId = "requestId";
}

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