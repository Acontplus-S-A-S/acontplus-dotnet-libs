namespace Acontplus.Core.Domain.Extensions;

public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this T value) => Result<T>.Success(value);

    public static Result<T> ToResult<T>(this DomainError error) => Result<T>.Failure(error);

    public static Result<T> ToResult<T>(this Exception exception) => Result<T>.Failure(DomainError.Internal("EXCEPTION", exception.Message));

    public static Result<T> ToResult<T>(this string errorMessage) => Result<T>.Failure(DomainError.Internal("ERROR", errorMessage));

    public static Result<T> ToResult<T>(this string errorCode, string errorMessage) => Result<T>.Failure(DomainError.Internal(errorCode, errorMessage));

    public static Result<T> ToResult<T>(this ErrorType errorType, string errorCode, string errorMessage) => Result<T>.Failure(new DomainError(errorType, errorCode, errorMessage));

    public static Result<T> ToResult<T>(this ErrorType errorType, string errorCode, string errorMessage, string? target) => Result<T>.Failure(new DomainError(errorType, errorCode, errorMessage, target));

    public static Result<T> ToResult<T>(this ErrorType errorType, string errorCode, string errorMessage, string? target, Dictionary<string, object>? details) => Result<T>.Failure(new DomainError(errorType, errorCode, errorMessage, target, details));

    public static Result<T> ToResult<T>(this DomainError error, T value) => error.Type switch
    {
        ErrorType.Validation => Result<T>.Failure(error),
        ErrorType.NotFound => Result<T>.Failure(error),
        ErrorType.Conflict => Result<T>.Failure(error),
        _ => Result<T>.Success(value)
    };

    public static Result<T> ToResult<T>(this bool condition, T value, DomainError error) => condition ? Result<T>.Success(value) : Result<T>.Failure(error);

    public static Result<T> ToResult<T>(this bool condition, T value, string errorCode, string errorMessage) => condition ? Result<T>.Success(value) : Result<T>.Failure(DomainError.Internal(errorCode, errorMessage));
}
