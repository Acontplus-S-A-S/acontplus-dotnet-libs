namespace Acontplus.Core.Domain.Common;

/// <summary>
/// Represents a result of an operation that can either succeed or fail.
/// Modern .NET 9+ pattern for functional error handling.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public readonly record struct Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;
    private readonly bool _isSuccess;

    private Result(TValue value)
    {
        _value = value;
        _error = default;
        _isSuccess = true;
    }

    private Result(TError error)
    {
        _value = default;
        _error = error;
        _isSuccess = false;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    public TValue Value => _isSuccess ? _value! : throw new InvalidOperationException("Cannot access Value on a failed result.");
    public TError Error => _isSuccess ? throw new InvalidOperationException("Cannot access Error on a successful result.") : _error!;

    public static Result<TValue, TError> Success(TValue value) => new(value);
    public static Result<TValue, TError> Failure(TError error) => new(error);

    public Result<TNewValue, TError> Map<TNewValue>(Func<TValue, TNewValue> mapper)
        where TNewValue : notnull
    {
        return _isSuccess ? Result<TNewValue, TError>.Success(mapper(_value!)) : Result<TNewValue, TError>.Failure(_error!);
    }

    public async Task<Result<TNewValue, TError>> MapAsync<TNewValue>(Func<TValue, Task<TNewValue>> mapper)
        where TNewValue : notnull
    {
        return _isSuccess ? Result<TNewValue, TError>.Success(await mapper(_value!)) : Result<TNewValue, TError>.Failure(_error!);
    }

    public Result<TValue, TNewError> MapError<TNewError>(Func<TError, TNewError> mapper)
        where TNewError : notnull
    {
        return _isSuccess ? Result<TValue, TNewError>.Success(_value!) : Result<TValue, TNewError>.Failure(mapper(_error!));
    }

    public Result<TNewValue, TNewError> MapBoth<TNewValue, TNewError>(
        Func<TValue, TNewValue> successMapper,
        Func<TError, TNewError> errorMapper)
        where TNewValue : notnull
        where TNewError : notnull
    {
        return _isSuccess
            ? Result<TNewValue, TNewError>.Success(successMapper(_value!))
            : Result<TNewValue, TNewError>.Failure(errorMapper(_error!));
    }

    public TValue Match(Func<TValue, TValue> success, Func<TError, TValue> failure)
    {
        return _isSuccess ? success(_value!) : failure(_error!);
    }

    public T Match<T>(Func<TValue, T> success, Func<TError, T> failure)
    {
        return _isSuccess ? success(_value!) : failure(_error!);
    }

    public async Task<T> MatchAsync<T>(Func<TValue, Task<T>> success, Func<TError, Task<T>> failure)
    {
        return _isSuccess ? await success(_value!) : await failure(_error!);
    }

    public void Match(Action<TValue> success, Action<TError> failure)
    {
        if (_isSuccess)
            success(_value!);
        else
            failure(_error!);
    }

    public async Task MatchAsync(Func<TValue, Task> success, Func<TError, Task> failure)
    {
        if (_isSuccess)
            await success(_value!);
        else
            await failure(_error!);
    }

    public Result<TValue, TError> OnSuccess(Action<TValue> action)
    {
        if (_isSuccess)
            action(_value!);
        return this;
    }

    public Result<TValue, TError> OnFailure(Action<TError> action)
    {
        if (!_isSuccess)
            action(_error!);
        return this;
    }

    public async Task<Result<TValue, TError>> OnSuccessAsync(Func<TValue, Task> action)
    {
        if (_isSuccess)
            await action(_value!);
        return this;
    }

    public async Task<Result<TValue, TError>> OnFailureAsync(Func<TError, Task> action)
    {
        if (!_isSuccess)
            await action(_error!);
        return this;
    }

    public static implicit operator Result<TValue, TError>(TValue value) => Success(value);
    public static implicit operator Result<TValue, TError>(TError error) => Failure(error);
}

/// <summary>
/// Represents a result of an operation that can either succeed or fail with a standard error type.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public readonly record struct Result<TValue> : IEquatable<Result<TValue>>
{
    private readonly TValue? _value;
    private readonly DomainError? _error;
    private readonly bool _isSuccess;

    private Result(TValue value)
    {
        _value = value;
        _error = null;
        _isSuccess = true;
    }

    private Result(DomainError error)
    {
        _value = default;
        _error = error;
        _isSuccess = false;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    public TValue Value => _isSuccess ? _value! : throw new InvalidOperationException("Cannot access Value on a failed result.");
    public DomainError Error => (DomainError)(_isSuccess ? throw new InvalidOperationException("Cannot access Error on a successful result.") : _error!);

    public static Result<TValue> Success(TValue value) => new(value);
    public static Result<TValue> Failure(DomainError error) => new(error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(DomainError error) => Failure(error);

    public T Match<T>(Func<TValue, T> success, Func<DomainError, T> failure)
    {
        return _isSuccess ? success(_value!) : failure(_error!.Value);
    }

    public async Task<T> MatchAsync<T>(Func<TValue, Task<T>> success, Func<DomainError, Task<T>> failure)
    {
        return _isSuccess ? await success(_value!) : await failure(_error!.Value);
    }

    public void Match(Action<TValue> success, Action<DomainError> failure)
    {
        if (_isSuccess)
            success(_value!);
        else
            failure(_error!.Value);
    }

    public async Task MatchAsync(Func<TValue, Task> success, Func<DomainError, Task> failure)
    {
        if (_isSuccess)
            await success(_value!);
        else
            await failure(_error!.Value);
    }

    public Result<TValue> OnSuccess(Action<TValue> action)
    {
        if (_isSuccess)
            action(_value!);
        return this;
    }

    public Result<TValue> OnFailure(Action<DomainError> action)
    {
        if (!_isSuccess)
            action(_error!.Value);
        return this;
    }

    public async Task<Result<TValue>> OnSuccessAsync(Func<TValue, Task> action)
    {
        if (_isSuccess)
            await action(_value!);
        return this;
    }

    public async Task<Result<TValue>> OnFailureAsync(Func<DomainError, Task> action)
    {
        if (!_isSuccess)
            await action(_error!.Value);
        return this;
    }

    public bool Equals(Result<TValue> other)
    {
        return _isSuccess == other._isSuccess &&
               EqualityComparer<TValue>.Default.Equals(_value, other._value) &&
               (_error ?? default).Equals(other._error ?? default);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_isSuccess, _value, _error);
    }
}