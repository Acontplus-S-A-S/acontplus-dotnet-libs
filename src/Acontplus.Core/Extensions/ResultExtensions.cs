namespace Acontplus.Core.Extensions;

/// <summary>
/// Modern extension methods for the Result pattern to improve developer experience.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Binds a Result to another Result using a function.
    /// </summary>
    public static Result<TNewValue> Bind<TValue, TNewValue>(
        this Result<TValue> result,
        Func<TValue, Result<TNewValue>> binder)
    {
        return result.Match(
            success: binder,
            failure: error => Result<TNewValue>.Failure(error)
        );
    }

    /// <summary>
    /// Binds a Result to another Result using an async function.
    /// </summary>
    public static async Task<Result<TNewValue>> BindAsync<TValue, TNewValue>(
        this Result<TValue> result,
        Func<TValue, Task<Result<TNewValue>>> binder)
    {
        return await result.MatchAsync(
            success: binder,
            failure: error => Task.FromResult(Result<TNewValue>.Failure(error))
        );
    }

    /// <summary>
    /// Maps a Result to another Result using a function.
    /// </summary>
    public static Result<TNewValue> Map<TValue, TNewValue>(
        this Result<TValue> result,
        Func<TValue, TNewValue> mapper)
    {
        return result.Match(
            success: value => Result<TNewValue>.Success(mapper(value)),
            failure: error => Result<TNewValue>.Failure(error)
        );
    }

    /// <summary>
    /// Maps a Result to another Result using an async function.
    /// </summary>
    public static async Task<Result<TNewValue>> MapAsync<TValue, TNewValue>(
        this Result<TValue> result,
        Func<TValue, Task<TNewValue>> mapper)
    {
        return await result.MatchAsync(
            success: async value => Result<TNewValue>.Success(await mapper(value)),
            failure: error => Task.FromResult(Result<TNewValue>.Failure(error))
        );
    }

    /// <summary>
    /// Executes an action on success and returns the original result.
    /// </summary>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        result.OnSuccess(action);
        return result;
    }

    /// <summary>
    /// Executes an async action on success and returns the original result.
    /// </summary>
    public static async Task<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action)
    {
        await result.OnSuccessAsync(action);
        return result;
    }

    /// <summary>
    /// Executes an action on failure and returns the original result.
    /// </summary>
    public static Result<T> TapError<T>(this Result<T> result, Action<DomainError> action)
    {
        result.OnFailure(action);
        return result;
    }

    /// <summary>
    /// Executes an async action on failure and returns the original result.
    /// </summary>
    public static async Task<Result<T>> TapErrorAsync<T>(this Result<T> result, Func<DomainError, Task> action)
    {
        await result.OnFailureAsync(action);
        return result;
    }

    /// <summary>
    /// Converts a nullable value to a Result.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, DomainError error) where T : class
    {
        return value is not null
            ? Result<T>.Success(value)
            : Result<T>.Failure(error);
    }

    /// <summary>
    /// Converts a nullable value to a Result with a custom error message.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, string errorCode, string errorMessage) where T : class
    {
        return value is not null
            ? Result<T>.Success(value)
            : Result<T>.Failure(DomainError.NotFound(errorCode, errorMessage));
    }

    /// <summary>
    /// Converts a boolean condition to a Result.
    /// </summary>
    public static Result<T> ToResult<T>(this bool condition, T value, DomainError error)
    {
        return condition
            ? Result<T>.Success(value)
            : Result<T>.Failure(error);
    }

    /// <summary>
    /// Converts a boolean condition to a Result with a custom error message.
    /// </summary>
    public static Result<T> ToResult<T>(this bool condition, T value, string errorCode, string errorMessage)
    {
        return condition
            ? Result<T>.Success(value)
            : Result<T>.Failure(DomainError.Validation(errorCode, errorMessage));
    }
}