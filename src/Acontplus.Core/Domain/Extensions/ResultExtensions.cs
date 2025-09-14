namespace Acontplus.Core.Domain.Extensions;

public static class ResultExtensions
{
    #region Value to Result

    public static Result<T> ToResult<T>(this T value) => Result<T>.Success(value);

    #endregion

    #region DomainError to Result

    public static Result<T> ToResult<T>(this DomainError error) => Result<T>.Failure(error);

    #endregion

    #region Exception to Result

    public static Result<T> ToResult<T>(this Exception exception) => Result<T>.Failure(DomainError.Internal("EXCEPTION", exception.Message));

    #endregion

    #region String to Result (Error Creation)

    public static Result<T> ToResult<T>(this string errorMessage) => Result<T>.Failure(DomainError.Internal("ERROR", errorMessage));

    public static Result<T> ToResult<T>(this (string code, string message) error) => Result<T>.Failure(DomainError.Internal(error.code, error.message));

    #endregion

    #region ErrorType to Result

    public static Result<T> ToResult<T>(this ErrorType errorType, string errorCode, string errorMessage) => Result<T>.Failure(new DomainError(errorType, errorCode, errorMessage));

    public static Result<T> ToResult<T>(this ErrorType errorType, string errorCode, string errorMessage, string? target) => Result<T>.Failure(new DomainError(errorType, errorCode, errorMessage, target));

    public static Result<T> ToResult<T>(this ErrorType errorType, string errorCode, string errorMessage, string? target, Dictionary<string, object>? details) => Result<T>.Failure(new DomainError(errorType, errorCode, errorMessage, target, details));

    #endregion

    #region Conditional Results

    [Obsolete("Passing an error with a value and returning success for non-fatal types is confusing. Use SuccessWithWarnings or return Result<SuccessWithWarnings<T>, DomainError>.")]
    public static Result<T> ToResult<T>(this DomainError error, T value) => error.Type switch
    {
        ErrorType.Validation => Result<T>.Failure(error),
        ErrorType.NotFound => Result<T>.Failure(error),
        ErrorType.Conflict => Result<T>.Failure(error),
        _ => Result<T>.Success(value)
    };

    public static Result<T> ToResult<T>(this bool condition, T value, DomainError error) => condition ? Result<T>.Success(value) : Result<T>.Failure(error);

    public static Result<T> ToResult<T>(this bool condition, T value, string errorCode, string errorMessage) => condition ? Result<T>.Success(value) : Result<T>.Failure(DomainError.Internal(errorCode, errorMessage));

    public static Result<T> ToResult<T>(this bool condition, T value, ErrorType errorType, string errorCode, string errorMessage) => condition ? Result<T>.Success(value) : Result<T>.Failure(new DomainError(errorType, errorCode, errorMessage));

    #endregion

    #region Fluent Factory Methods for Common Error Types

    public static Result<T> ValidationError<T>(string code, string message, string? target = null, Dictionary<string, object>? details = null) =>
        Result<T>.Failure(DomainError.Validation(code, message, target, details));

    public static Result<T> NotFoundError<T>(string code, string message, string? target = null, Dictionary<string, object>? details = null) =>
        Result<T>.Failure(DomainError.NotFound(code, message, target, details));

    public static Result<T> ConflictError<T>(string code, string message, string? target = null, Dictionary<string, object>? details = null) =>
        Result<T>.Failure(DomainError.Conflict(code, message, target, details));

    public static Result<T> UnauthorizedError<T>(string code, string message, string? target = null, Dictionary<string, object>? details = null) =>
        Result<T>.Failure(DomainError.Unauthorized(code, message, target, details));

    public static Result<T> ForbiddenError<T>(string code, string message, string? target = null, Dictionary<string, object>? details = null) =>
        Result<T>.Failure(DomainError.Forbidden(code, message, target, details));

    public static Result<T> InternalError<T>(string code, string message, string? target = null, Dictionary<string, object>? details = null) =>
        Result<T>.Failure(DomainError.Internal(code, message, target, details));

    #endregion

    #region Success With Warnings helpers

    public static Result<SuccessWithWarnings<T>> ToSuccessWithWarningsResult<T>(this T value, DomainWarnings warnings)
        => Result<SuccessWithWarnings<T>>.Success(SuccessWithWarnings<T>.Create(value, warnings));

    public static Result<SuccessWithWarnings<T>> ToSuccessWithWarningsResult<T>(this T value, params DomainError[] warnings)
        => Result<SuccessWithWarnings<T>>.Success(SuccessWithWarnings<T>.Create(value, warnings));

    #endregion

    #region DomainErrors to Result helpers

    // Create Result<T, DomainErrors> failure from DomainErrors
    public static Result<T, DomainErrors> ToFailureResult<T>(this DomainErrors errors)
        => Result<T, DomainErrors>.Failure(errors);

    // Create Result<T, DomainErrors> failure from IEnumerable<DomainError>
    public static Result<T, DomainErrors> ToFailureResult<T>(this IEnumerable<DomainError> errors)
        => Result<T, DomainErrors>.Failure(new DomainErrors(errors.ToList()));

    // Create Result<T, DomainErrors> failure from params
    public static Result<T, DomainErrors> ToFailureResult<T>(params DomainError[] errors)
        => Result<T, DomainErrors>.Failure(new DomainErrors(errors));

    #endregion
}
