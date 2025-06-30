namespace Acontplus.Core.DTOs;

/// <summary>
/// Standardized API response format for modern web applications
/// </summary>
/// <typeparam name="T">Type of the payload data</typeparam>
public sealed class ApiResponse<T>
{
    public required string Status { get; init; }
    public string? Code { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public IEnumerable<ApiError>? Errors { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }

    public static ApiResponse<T> Success(
        T data,
        string? message = null,
        string? code = null,
        Dictionary<string, object>? metadata = null)
    {
        return new ApiResponse<T>
        {
            Status = "success",
            Code = code ?? "200",
            Message = message ?? "Operation completed successfully",
            Data = data,
            Metadata = metadata
        };
    }

    public static ApiResponse<T> Failure(
        IEnumerable<ApiError> errors,
        string? message = "One or more errors occurred",
        string? code = "400")
    {
        return new ApiResponse<T>
        {
            Status = "error",
            Code = code,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse<T> Failure(
        ApiError error,
        string? message = "An error occurred",
        string? code = "400")
    {
        return Failure(new[] { error }, message, code);
    }
}

/// <summary>
/// Non-generic version for operations without return data
/// </summary>
public sealed class ApiResponse
{
    public required string Status { get; init; }
    public string? Code { get; init; }
    public string? Message { get; init; }
    public IEnumerable<ApiError>? Errors { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }

    public static ApiResponse Success(
        string? message = null,
        string? code = null,
        Dictionary<string, object>? metadata = null)
    {
        return new ApiResponse
        {
            Status = "success",
            Code = code ?? "200",
            Message = message ?? "Operation completed successfully",
            Metadata = metadata
        };
    }

    public static ApiResponse Failure(
        IEnumerable<ApiError> errors,
        string? message = "One or more errors occurred",
        string? code = "400")
    {
        return new ApiResponse
        {
            Status = "error",
            Code = code,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse Failure(
        ApiError error,
        string? message = "An error occurred",
        string? code = "400")
    {
        return Failure(new[] { error }, message, code);
    }
}

/// <summary>
/// Standardized error format
/// </summary>
public sealed record ApiError(
    string Code,
    string Message,
    string? Target = null,
    Dictionary<string, string>? Details = null);