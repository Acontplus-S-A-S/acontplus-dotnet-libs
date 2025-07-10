using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

/// <summary>
/// Configuration object for creating API responses (input only)
/// </summary>
public sealed class ApiResponseOptions
{
    public string? Message { get; init; }
    public IReadOnlyList<ApiError>? Errors { get; init; }
    public IReadOnlyList<ApiError>? Warnings { get; init; }
    public IReadOnlyDictionary<string, object>? Metadata { get; init; }
    public string? CorrelationId { get; init; }
    public string? TraceId { get; init; }
    public string? Timestamp { get; init; }
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
}

/// <summary>
/// Standard API response wrapper for all endpoints
/// </summary>
public record ApiResponse<T>
{
    public ResponseStatus Status { get; init; }
    public string Code { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ApiError>? Errors { get; init; }
    public IReadOnlyList<ApiError>? Warnings { get; init; }
    public IReadOnlyDictionary<string, object>? Metadata { get; init; }
    public string? CorrelationId { get; init; }
    public string? TraceId { get; init; }
    public string Timestamp { get; init; }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; init; }

    // Primary constructor with all properties
    public ApiResponse(
        ResponseStatus status,
        string code,
        T? data,
        string? message,
        IReadOnlyList<ApiError>? errors,
        IReadOnlyList<ApiError>? warnings,
        IReadOnlyDictionary<string, object>? metadata,
        string? correlationId,
        string? traceId,
        string? timestamp,
        HttpStatusCode statusCode)
    {
        Status = status;
        Code = code;
        Data = data;
        Message = message;
        Errors = errors;
        Warnings = warnings;
        Metadata = metadata;
        CorrelationId = correlationId;
        TraceId = traceId ?? Activity.Current?.Id;
        Timestamp = timestamp ?? DateTimeOffset.UtcNow.ToString("O");
        StatusCode = statusCode;
    }

    [JsonIgnore] public bool IsSuccess => Status == ResponseStatus.Success;
    [JsonIgnore] public bool IsError => Status == ResponseStatus.Error;
    [JsonIgnore] public bool HasWarnings => Warnings?.Count > 0;
    [JsonIgnore] public bool HasErrors => Errors?.Count > 0;
    [JsonIgnore] public bool HasData => Data is not null;

    // Factory methods using options pattern
    public static ApiResponse<T> Success(T data, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.OK;
        return new ApiResponse<T>(
            status: ResponseStatus.Success,
            code: ((int)statusCode).ToString(),
            data: data,
            message: options?.Message ?? "Operation completed successfully.",
            errors: Array.Empty<ApiError>(),
            warnings: options?.Warnings,
            metadata: options?.Metadata,
            correlationId: options?.CorrelationId,
            traceId: options?.TraceId,
            timestamp: options?.Timestamp,
            statusCode: statusCode
        );
    }

    public static ApiResponse<T> Failure(ApiError error, ApiResponseOptions? options = null)
        => Failure(new[] { error }, options);

    public static ApiResponse<T> Failure(IReadOnlyList<ApiError>? errors, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.BadRequest;
        return new ApiResponse<T>(
            status: ResponseStatus.Error,
            code: ((int)statusCode).ToString(),
            data: default,
            message: options?.Message ?? "An error occurred.",
            errors: errors,
            warnings: options?.Warnings,
            metadata: options?.Metadata,
            correlationId: options?.CorrelationId,
            traceId: options?.TraceId,
            timestamp: options?.Timestamp,
            statusCode: statusCode
        );
    }

    public static ApiResponse<T> Warning(T data, IReadOnlyList<ApiError> warnings, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.OK;
        return new ApiResponse<T>(
            status: ResponseStatus.Warning,
            code: ((int)statusCode).ToString(),
            data: data,
            message: options?.Message ?? "Operation completed with warnings.",
            errors: Array.Empty<ApiError>(),
            warnings: warnings,
            metadata: options?.Metadata,
            correlationId: options?.CorrelationId,
            traceId: options?.TraceId,
            timestamp: options?.Timestamp,
            statusCode: statusCode
        );
    }
}

// Non-generic version
public sealed record ApiResponse : ApiResponse<object?>
{
    private ApiResponse(
        ResponseStatus status,
        string code,
        object? data,
        string? message,
        IReadOnlyList<ApiError>? errors,
        IReadOnlyList<ApiError>? warnings,
        IReadOnlyDictionary<string, object>? metadata,
        string? correlationId,
        string? traceId,
        string? timestamp,
        HttpStatusCode statusCode)
        : base(status, code, data, message, errors, warnings, metadata,
              correlationId, traceId, timestamp, statusCode)
    {
    }

    public static new ApiResponse Success(ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.OK;
        return new ApiResponse(
            status: ResponseStatus.Success,
            code: ((int)statusCode).ToString(),
            data: null,
            message: options?.Message ?? "Operation completed successfully.",
            errors: Array.Empty<ApiError>(),
            warnings: options?.Warnings,
            metadata: options?.Metadata,
            correlationId: options?.CorrelationId,
            traceId: options?.TraceId,
            timestamp: options?.Timestamp,
            statusCode: statusCode
        );
    }

    public static new ApiResponse Failure(IReadOnlyList<ApiError> errors, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.BadRequest;
        return new ApiResponse(
            status: ResponseStatus.Error,
            code: ((int)statusCode).ToString(),
            data: null,
            message: options?.Message ?? "An error occurred.",
            errors: errors,
            warnings: options?.Warnings,
            metadata: options?.Metadata,
            correlationId: options?.CorrelationId,
            traceId: options?.TraceId,
            timestamp: options?.Timestamp,
            statusCode: statusCode
        );
    }

    public static ApiResponse Failure(ApiError error, ApiResponseOptions? options = null)
        => Failure(new[] { error }, options);
}