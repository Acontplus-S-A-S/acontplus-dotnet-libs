using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

/// <summary>
/// Configuration object for creating API responses.
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
/// Standard API response wrapper for all endpoints.
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
    public string Timestamp { get; init; } = null!;
    
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    // Primary constructor with reduced parameters
    public ApiResponse(ResponseStatus status, string code, T? data, ApiResponseOptions? options = null)
    {
        Status = status;
        Code = code;
        Data = data;
        Message = options?.Message;
        Errors = options?.Errors;
        Warnings = options?.Warnings;
        Metadata = options?.Metadata;
        CorrelationId = options?.CorrelationId;
        TraceId = options?.TraceId ?? Activity.Current?.Id;
        Timestamp = options?.Timestamp ?? DateTimeOffset.UtcNow.ToString("O");
        StatusCode = options?.StatusCode ?? HttpStatusCode.OK;
    }

    [JsonIgnore] public bool IsSuccess => Status == ResponseStatus.Success;
    [JsonIgnore] public bool IsError => Status == ResponseStatus.Error;
    [JsonIgnore] public bool HasWarnings => Warnings?.Count > 0;
    [JsonIgnore] public bool HasErrors => Errors?.Count > 0;
    [JsonIgnore] public bool HasData => Data is not null;

    // --- Static Factory/Builder Methods ---

    public static ApiResponse<T> Success(T data, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.OK;
        var responseOptions = new ApiResponseOptions
        {
            Message = options?.Message ?? "Operation completed successfully.",
            Errors = Array.Empty<ApiError>(),
            Warnings = options?.Warnings,
            Metadata = options?.Metadata,
            CorrelationId = options?.CorrelationId,
            TraceId = options?.TraceId,
            Timestamp = options?.Timestamp,
            StatusCode = statusCode
        };

        return new ApiResponse<T>(ResponseStatus.Success, ((int)statusCode).ToString(), data, responseOptions);
    }

    public static ApiResponse<T> Failure(ApiError error, ApiResponseOptions? options = null)
    => Failure(new[] { error }, options);

    public static ApiResponse<T> Failure(IReadOnlyList<ApiError>? errors, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.BadRequest;
        var responseOptions = new ApiResponseOptions
        {
            Message = options?.Message ?? "An error occurred.",
            Errors = errors,
            Warnings = options?.Warnings,
            Metadata = options?.Metadata,
            CorrelationId = options?.CorrelationId,
            TraceId = options?.TraceId,
            Timestamp = options?.Timestamp,
            StatusCode = statusCode
        };

        return new ApiResponse<T>(ResponseStatus.Error, ((int)statusCode).ToString(), default, responseOptions);
    }

    public static ApiResponse<T> Warning(T data, IReadOnlyList<ApiError> warnings, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.OK;
        var responseOptions = new ApiResponseOptions
        {
            Message = options?.Message ?? "Operation completed with warnings.",
            Errors = Array.Empty<ApiError>(),
            Warnings = warnings,
            Metadata = options?.Metadata,
            CorrelationId = options?.CorrelationId,
            TraceId = options?.TraceId,
            Timestamp = options?.Timestamp,
            StatusCode = statusCode
        };

        return new ApiResponse<T>(ResponseStatus.Warning, ((int)statusCode).ToString(), data, responseOptions);
    }
}

// Non-generic version for responses without data
public sealed record ApiResponse(ResponseStatus Status, string Code, object? Data = null, ApiResponseOptions? Options = null)
    : ApiResponse<object?>(Status, Code, Data, Options)
{
    // Static factory methods for non-generic version
    public static ApiResponse Success(ApiResponseOptions? options = null) =>
        new(ResponseStatus.Success, ((int)(options?.StatusCode ?? HttpStatusCode.OK)).ToString(), null, options);

    public static ApiResponse Failure(IReadOnlyList<ApiError> errors, ApiResponseOptions? options = null)
    {
        var statusCode = options?.StatusCode ?? HttpStatusCode.BadRequest;
        var responseOptions = new ApiResponseOptions
        {
            Message = options?.Message ?? "An error occurred.",
            Errors = errors,
            Warnings = options?.Warnings,
            Metadata = options?.Metadata,
            CorrelationId = options?.CorrelationId,
            TraceId = options?.TraceId,
            Timestamp = options?.Timestamp,
            StatusCode = statusCode
        };

        return new ApiResponse(ResponseStatus.Error, ((int)statusCode).ToString(), null, responseOptions);
    }

    public static ApiResponse Failure(ApiError error, ApiResponseOptions? options = null)
    => Failure(new[] { error }, options);
}