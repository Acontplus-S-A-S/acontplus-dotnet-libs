using System.Net;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ApiResponse<>))]
public partial class ApiResponseJsonContext : JsonSerializerContext { }

public record ApiResponse
{
    public required string Status { get; init; }
    public required string Code { get; init; }
    public string? Message { get; init; }
    public IEnumerable<ApiError>? Errors { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
    public string Timestamp { get; init; } = DateTimeOffset.UtcNow.ToString("O");
    public string? CorrelationId { get; init; }
    public string? TraceId { get; init; } = System.Diagnostics.Activity.Current?.Id;

    // Success Factories
    public static ApiResponse Success(
            string? message = null,
            Dictionary<string, object>? metadata = null,
            string? correlationId = null,
            HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse
        {
            Status = "success",
            Code = ((int)statusCode).ToString(),
            Message = message ?? ApiResponseHelpers.GetDefaultSuccessMessage(statusCode),
            Metadata = metadata,
            CorrelationId = correlationId
        };
    }

    // Failure Factories
    public static ApiResponse Failure(
        IEnumerable<ApiError> errors,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse
        {
            Status = "error",
            Code = ((int)statusCode).ToString(),
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

    // Warning Factory
    public static ApiResponse Warning(
        IEnumerable<ApiError> warnings,
        string? message = "Operation completed with warnings",
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse
        {
            Status = "warning",
            Code = ((int)statusCode).ToString(),
            Message = message,
            Errors = warnings,
            CorrelationId = correlationId
        };
    }

    [JsonIgnore] public bool IsSuccess => Status == "success";
    [JsonIgnore] public bool IsError => Status == "error";
    [JsonIgnore] public bool HasWarnings => Status == "warning";
    [JsonIgnore] public bool HasErrors => Errors?.Any() == true;
}

public record ApiResponse<T> : ApiResponse
{
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    // Success Factories
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
            Code = ((int)statusCode).ToString(),
            Message = message ?? ApiResponseHelpers.GetDefaultSuccessMessage(statusCode),
            Data = data,
            Metadata = metadata,
            CorrelationId = correlationId
        };
    }

    // Failure Factories
    public static new ApiResponse<T> Failure(
        IEnumerable<ApiError> errors,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T>
        {
            Status = "error",
            Code = ((int)statusCode).ToString(),
            Message = message ?? ApiResponseHelpers.GetDefaultErrorMessage(statusCode),
            Errors = errors,
            CorrelationId = correlationId
        };
    }

    public static new ApiResponse<T> Failure(
        ApiError error,
        string? message = null,
        string? correlationId = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return Failure([error], message, correlationId, statusCode);
    }

    // Warning Factory
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
            Code = ((int)statusCode).ToString(),
            Message = message,
            Data = data,
            Errors = warnings,
            CorrelationId = correlationId
        };
    }

    [JsonIgnore] public bool HasData => Data is not null;
}