using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ApiResponse<>))]
public partial class ApiResponseJsonContext : JsonSerializerContext { }

public record ApiResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ResponseStatus Status { get; init; }
    public required string Code { get; init; }
    public string? Message { get; init; }
    public IEnumerable<ApiError>? Errors { get; init; }
    public IEnumerable<ApiError>? Warnings { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
    public string Timestamp { get; init; } = DateTimeOffset.UtcNow.ToString("O");
    public string? CorrelationId { get; init; }
    public string? TraceId { get; init; } = Activity.Current?.Id;

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    // Factory methods
    protected static ApiResponse Create(
        ResponseStatus status,
        string code,
        string? message = null,
        IEnumerable<ApiError>? errors = null,
        IEnumerable<ApiError>? warnings = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse
        {
            Status = status,
            Code = code,
            Message = message,
            Errors = errors,
            Warnings = warnings,
            Metadata = metadata,
            CorrelationId = correlationId,
            StatusCode = statusCode
        };
    }

    public static ApiResponse Success(
        string? message = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return Create(
            status: ResponseStatus.Success,
            code: ((int)statusCode).ToString(),
            message: message ?? ApiResponseHelpers.GetDefaultSuccessMessage(statusCode),
            metadata: metadata,
            correlationId: correlationId,
            statusCode: statusCode
        );
    }

    public static ApiResponse Failure(
        IEnumerable<ApiError> errors,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        Dictionary<string, object>? metadata = null,
        IEnumerable<ApiError>? warnings = null)
    {
        return Create(
            status: ResponseStatus.Error,
            code: ((int)statusCode).ToString(),
            message: message ?? ApiResponseHelpers.GetDefaultErrorMessage(statusCode),
            errors: errors,
            warnings: warnings,
            metadata: metadata,
            correlationId: correlationId,
            statusCode: statusCode
        );
    }

    public static ApiResponse Failure(
        ApiError error,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        Dictionary<string, object>? metadata = null,
        IEnumerable<ApiError>? warnings = null)
    {
        return Failure([error], message, correlationId, statusCode, metadata, warnings);
    }

    public static ApiResponse Warning(
        IEnumerable<ApiError> warnings,
        string? message = "Operation completed with warnings",
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        Dictionary<string, object>? metadata = null)
    {
        return Create(
            status: ResponseStatus.Warning,
            code: ((int)statusCode).ToString(),
            message: message,
            warnings: warnings,
            metadata: metadata,
            correlationId: correlationId,
            statusCode: statusCode
        );
    }

    [JsonIgnore] public bool IsSuccess => Status == ResponseStatus.Success;
    [JsonIgnore] public bool IsError => Status == ResponseStatus.Error;
    [JsonIgnore] public bool HasWarnings => Warnings?.Any() == true;
    [JsonIgnore] public bool HasErrors => Errors?.Any() == true;
}

public record ApiResponse<T> : ApiResponse
{
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    private new static ApiResponse<T> Create(
        ResponseStatus status,
        string code,
        T? data = default,
        string? message = null,
        IEnumerable<ApiError>? errors = null,
        IEnumerable<ApiError>? warnings = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse<T>
        {
            Status = status,
            Code = code,
            Data = data,
            Message = message,
            Errors = errors,
            Warnings = warnings,
            Metadata = metadata,
            CorrelationId = correlationId,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Success(
        T data,
        string? message = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        IEnumerable<ApiError>? warnings = null)
    {
        return Create(
            status: ResponseStatus.Success,
            code: ((int)statusCode).ToString(),
            data: data,
            message: message ?? ApiResponseHelpers.GetDefaultSuccessMessage(statusCode),
            warnings: warnings,
            metadata: metadata,
            correlationId: correlationId,
            statusCode: statusCode
        );
    }

    public static new ApiResponse<T> Failure(
        IEnumerable<ApiError> errors,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        Dictionary<string, object>? metadata = null,
        IEnumerable<ApiError>? warnings = null)
    {
        return Create(
            status: ResponseStatus.Error,
            code: ((int)statusCode).ToString(),
            message: message ?? ApiResponseHelpers.GetDefaultErrorMessage(statusCode),
            errors: errors,
            warnings: warnings,
            metadata: metadata,
            correlationId: correlationId,
            statusCode: statusCode
        );
    }

    public static new ApiResponse<T> Failure(
        ApiError error,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        Dictionary<string, object>? metadata = null,
        IEnumerable<ApiError>? warnings = null)
    {
        return Failure([error], message, correlationId, statusCode, metadata, warnings);
    }

    public static ApiResponse<T> Warning(
        T data,
        IEnumerable<ApiError> warnings,
        string? message = "Operation completed with warnings",
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        Dictionary<string, object>? metadata = null)
    {
        return Create(
            status: ResponseStatus.Warning,
            code: ((int)statusCode).ToString(),
            data: data,
            message: message,
            warnings: warnings,
            metadata: metadata,
            correlationId: correlationId,
            statusCode: statusCode
        );
    }

    [JsonIgnore] public bool HasData => Data is not null;
}