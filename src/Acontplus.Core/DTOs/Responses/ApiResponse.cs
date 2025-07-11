using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace Acontplus.Core.DTOs.Responses;

public sealed record ApiResponseOptions
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

public record ApiResponse<T>
{
    public ResponseStatus Status { get; }
    public string Code { get; }
    public T? Data { get; }
    public string? Message { get; }
    public IReadOnlyList<ApiError>? Errors { get; }
    public IReadOnlyList<ApiError>? Warnings { get; }
    public IReadOnlyDictionary<string, object>? Metadata { get; }
    public string? CorrelationId { get; }
    public string? TraceId { get; }
    public string Timestamp { get; }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; }

    // Changed to protected internal to allow inheritance
    protected internal ApiResponse(
        ResponseStatus status,
        string code,
        ApiResponseOptions options,
        T? data = default)
    {
        Status = status;
        Code = code;
        Data = data;
        Message = options.Message;
        Errors = options.Errors;
        Warnings = options.Warnings;
        Metadata = options.Metadata;
        CorrelationId = options.CorrelationId;
        TraceId = options.TraceId ?? Activity.Current?.Id;
        Timestamp = options.Timestamp ?? DateTimeOffset.UtcNow.ToString("O");
        StatusCode = options.StatusCode;
    }

    [JsonIgnore] public bool IsSuccess => Status == ResponseStatus.Success;
    [JsonIgnore] public bool IsError => Status == ResponseStatus.Error;
    [JsonIgnore] public bool HasWarnings => Warnings?.Count > 0;
    [JsonIgnore] public bool HasErrors => Errors?.Count > 0;
    [JsonIgnore] public bool HasData => Data is not null;

    public static ApiResponse<T> Success(T data, ApiResponseOptions? options = null)
    {
        options = InitializeOptions(options, HttpStatusCode.OK);
        return new ApiResponse<T>(
            ResponseStatus.Success,
            ((int)options.StatusCode).ToString(),
            new ApiResponseOptions
            {
                Message = options.Message ?? "Operation completed successfully.",
                Errors = Array.Empty<ApiError>(),
                Warnings = options.Warnings,
                Metadata = options.Metadata,
                CorrelationId = options.CorrelationId,
                TraceId = options.TraceId,
                Timestamp = options.Timestamp,
                StatusCode = options.StatusCode
            },
            data);
    }

    public static ApiResponse<T> Failure(ApiError error, ApiResponseOptions? options = null)
        => Failure(new[] { error }, options);

    public static ApiResponse<T> Failure(IReadOnlyList<ApiError>? errors, ApiResponseOptions? options = null)
    {
        options = InitializeOptions(options, HttpStatusCode.BadRequest);
        return new ApiResponse<T>(
            ResponseStatus.Error,
            ((int)options.StatusCode).ToString(),
            new ApiResponseOptions
            {
                Message = options.Message ?? "An error occurred.",
                Errors = errors ?? Array.Empty<ApiError>(),
                Warnings = options.Warnings,
                Metadata = options.Metadata,
                CorrelationId = options.CorrelationId,
                TraceId = options.TraceId,
                Timestamp = options.Timestamp,
                StatusCode = options.StatusCode
            });
    }

    public static ApiResponse<T> Warning(T data, IReadOnlyList<ApiError> warnings, ApiResponseOptions? options = null)
    {
        options = InitializeOptions(options, HttpStatusCode.OK);
        return new ApiResponse<T>(
            ResponseStatus.Warning,
            ((int)options.StatusCode).ToString(),
            new ApiResponseOptions
            {
                Message = options.Message ?? "Operation completed with warnings.",
                Errors = Array.Empty<ApiError>(),
                Warnings = warnings,
                Metadata = options.Metadata,
                CorrelationId = options.CorrelationId,
                TraceId = options.TraceId,
                Timestamp = options.Timestamp,
                StatusCode = options.StatusCode
            },
            data);
    }

    private static ApiResponseOptions InitializeOptions(ApiResponseOptions? options, HttpStatusCode defaultStatusCode)
    {
        if (options == null)
        {
            return new ApiResponseOptions { StatusCode = defaultStatusCode };
        }

        if (options.StatusCode == HttpStatusCode.OK && defaultStatusCode != HttpStatusCode.OK)
        {
            return options with { StatusCode = defaultStatusCode };
        }

        return options;
    }
}

public sealed record ApiResponse : ApiResponse<object?>
{
    private ApiResponse(
        ResponseStatus status,
        string code,
        ApiResponseOptions options,
        object? data = null)
        : base(status, code, options, data)
    {
    }

    public static new ApiResponse Success(object? data = null, ApiResponseOptions? options = null)
    {
        options = InitializeOptions(options, HttpStatusCode.OK);
        return new ApiResponse(
            ResponseStatus.Success,
            ((int)options.StatusCode).ToString(),
            new ApiResponseOptions
            {
                Message = options.Message ?? "Operation completed successfully.",
                Errors = Array.Empty<ApiError>(),
                Warnings = options.Warnings,
                Metadata = options.Metadata,
                CorrelationId = options.CorrelationId,
                TraceId = options.TraceId,
                Timestamp = options.Timestamp,
                StatusCode = options.StatusCode
            },
            data
        );
    }

    public static new ApiResponse Failure(IReadOnlyList<ApiError> errors, ApiResponseOptions? options = null)
    {
        options = InitializeOptions(options, HttpStatusCode.BadRequest);
        return new ApiResponse(
            ResponseStatus.Error,
            ((int)options.StatusCode).ToString(),
            new ApiResponseOptions
            {
                Message = options.Message ?? "An error occurred.",
                Errors = errors,
                Warnings = options.Warnings,
                Metadata = options.Metadata,
                CorrelationId = options.CorrelationId,
                TraceId = options.TraceId,
                Timestamp = options.Timestamp,
                StatusCode = options.StatusCode
            });
    }

    public static new ApiResponse Failure(ApiError error, ApiResponseOptions? options = null)
        => Failure(new[] { error }, options);

    private static ApiResponseOptions InitializeOptions(ApiResponseOptions? options, HttpStatusCode defaultStatusCode)
    {
        if (options == null)
        {
            return new ApiResponseOptions { StatusCode = defaultStatusCode };
        }

        if (options.StatusCode == HttpStatusCode.OK && defaultStatusCode != HttpStatusCode.OK)
        {
            return new ApiResponseOptions
            {
                Message = options.Message,
                Errors = options.Errors,
                Warnings = options.Warnings,
                Metadata = options.Metadata,
                CorrelationId = options.CorrelationId,
                TraceId = options.TraceId,
                Timestamp = options.Timestamp,
                StatusCode = defaultStatusCode
            };
        }

        return options;
    }
}