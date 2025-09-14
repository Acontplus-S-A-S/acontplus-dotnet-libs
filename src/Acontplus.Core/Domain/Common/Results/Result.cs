namespace Acontplus.Core.Domain.Common.Results;

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

    #region Functional Composition Methods

    /// <summary>
    /// Maps the value of a successful result to a new type.
    /// </summary>
    public Result<TNewValue, TError> Map<TNewValue>(Func<TValue, TNewValue> mapper)
        where TNewValue : notnull
    {
        return _isSuccess ? Result<TNewValue, TError>.Success(mapper(_value!)) : Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Asynchronously maps the value of a successful result to a new type.
    /// </summary>
    public async Task<Result<TNewValue, TError>> MapAsync<TNewValue>(Func<TValue, Task<TNewValue>> mapper)
        where TNewValue : notnull
    {
        return _isSuccess ? Result<TNewValue, TError>.Success(await mapper(_value!)) : Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Maps the error of a failed result to a new error type.
    /// </summary>
    public Result<TValue, TNewError> MapError<TNewError>(Func<TError, TNewError> mapper)
        where TNewError : notnull
    {
        return _isSuccess ? Result<TValue, TNewError>.Success(_value!) : Result<TValue, TNewError>.Failure(mapper(_error!));
    }

    /// <summary>
    /// Maps both success and failure cases to new types.
    /// </summary>
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

    /// <summary>
    /// Binds (flat maps) the result with a function that returns another Result.
    /// This is the core method for chaining operations that can fail.
    /// </summary>
    public Result<TNewValue, TError> Bind<TNewValue>(Func<TValue, Result<TNewValue, TError>> binder)
        where TNewValue : notnull
    {
        return _isSuccess ? binder(_value!) : Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Asynchronously binds the result with a function that returns another Result.
    /// </summary>
    public async Task<Result<TNewValue, TError>> BindAsync<TNewValue>(Func<TValue, Task<Result<TNewValue, TError>>> binder)
        where TNewValue : notnull
    {
        return _isSuccess ? await binder(_value!) : Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Applies a Result-wrapped function to a Result-wrapped value (Applicative pattern).
    /// </summary>
    public Result<TNewValue, TError> Apply<TNewValue>(Result<Func<TValue, TNewValue>, TError> functionResult)
        where TNewValue : notnull
    {
        return functionResult.IsSuccess && _isSuccess
            ? Result<TNewValue, TError>.Success(functionResult.Value(_value!))
            : functionResult.IsFailure
                ? Result<TNewValue, TError>.Failure(functionResult.Error)
                : Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Filters the result with a predicate. Returns failure if predicate is false.
    /// </summary>
    public Result<TValue, TError> Filter(Func<TValue, bool> predicate, TError errorOnFalse)
    {
        return _isSuccess && predicate(_value!)
            ? this
            : _isSuccess
                ? Result<TValue, TError>.Failure(errorOnFalse)
                : this;
    }

    /// <summary>
    /// Filters the result with a predicate. Returns failure if predicate is false.
    /// </summary>
    public Result<TValue, TError> Filter(Func<TValue, bool> predicate, Func<TValue, TError> errorFactory)
    {
        return _isSuccess && predicate(_value!)
            ? this
            : _isSuccess
                ? Result<TValue, TError>.Failure(errorFactory(_value!))
                : this;
    }

    /// <summary>
    /// Returns the current result if successful, otherwise returns the alternative result.
    /// </summary>
    public Result<TValue, TError> Or(Result<TValue, TError> alternative)
    {
        return _isSuccess ? this : alternative;
    }

    /// <summary>
    /// Returns the current result if successful, otherwise returns the result from the alternative function.
    /// </summary>
    public Result<TValue, TError> Or(Func<TError, Result<TValue, TError>> alternativeFactory)
    {
        return _isSuccess ? this : alternativeFactory(_error!);
    }

    /// <summary>
    /// Recovers from a failure by providing a fallback value.
    /// </summary>
    public Result<TValue, TError> Recover(TValue fallbackValue)
    {
        return _isSuccess ? this : Result<TValue, TError>.Success(fallbackValue);
    }

    /// <summary>
    /// Recovers from a failure by using a function to provide a fallback value.
    /// </summary>
    public Result<TValue, TError> Recover(Func<TError, TValue> fallbackFactory)
    {
        return _isSuccess ? this : Result<TValue, TError>.Success(fallbackFactory(_error!));
    }

    /// <summary>
    /// Recovers from a failure by using a function that returns a Result.
    /// </summary>
    public Result<TValue, TError> RecoverWith(Func<TError, Result<TValue, TError>> recoveryFactory)
    {
        return _isSuccess ? this : recoveryFactory(_error!);
    }

    /// <summary>
    /// Taps into the success value without changing the result (for side effects).
    /// </summary>
    public Result<TValue, TError> Tap(Action<TValue> action)
    {
        if (_isSuccess)
            action(_value!);
        return this;
    }

    /// <summary>
    /// Asynchronously taps into the success value without changing the result.
    /// </summary>
    public async Task<Result<TValue, TError>> TapAsync(Func<TValue, Task> action)
    {
        if (_isSuccess)
            await action(_value!);
        return this;
    }

    /// <summary>
    /// Taps into the error without changing the result (for side effects).
    /// </summary>
    public Result<TValue, TError> TapError(Action<TError> action)
    {
        if (!_isSuccess)
            action(_error!);
        return this;
    }

    /// <summary>
    /// Asynchronously taps into the error without changing the result.
    /// </summary>
    public async Task<Result<TValue, TError>> TapErrorAsync(Func<TError, Task> action)
    {
        if (!_isSuccess)
            await action(_error!);
        return this;
    }

    #endregion

    #region Pattern Matching Methods

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

    #endregion

    #region Side Effect Methods (Backwards Compatibility)

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

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the value if successful, otherwise returns the default value.
    /// </summary>
    public TValue GetValueOrDefault(TValue defaultValue = default!)
    {
        return _isSuccess ? _value! : defaultValue;
    }

    /// <summary>
    /// Gets the value if successful, otherwise returns the result of the factory function.
    /// </summary>
    public TValue GetValueOrDefault(Func<TError, TValue> defaultFactory)
    {
        return _isSuccess ? _value! : defaultFactory(_error!);
    }

    /// <summary>
    /// Throws an exception if the result is a failure.
    /// </summary>
    public TValue ThrowOnFailure()
    {
        return _isSuccess 
            ? _value! 
            : throw new InvalidOperationException($"Result failed with error: {_error}");
    }

    /// <summary>
    /// Throws a custom exception if the result is a failure.
    /// </summary>
    public TValue ThrowOnFailure<TException>(Func<TError, TException> exceptionFactory)
        where TException : Exception
    {
        return _isSuccess 
            ? _value! 
            : throw exceptionFactory(_error!);
    }

    #endregion

    public static implicit operator Result<TValue, TError>(TValue value) => Success(value);
    public static implicit operator Result<TValue, TError>(TError error) => Failure(error);
}

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

    #region Functional Composition Methods

    /// <summary>
    /// Maps the value of a successful result to a new type.
    /// </summary>
    public Result<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> mapper)
        where TNewValue : notnull
    {
        return _isSuccess 
            ? Result<TNewValue>.Success(mapper(_value!)) 
            : Result<TNewValue>.Failure(_error!.Value);
    }

    /// <summary>
    /// Asynchronously maps the value of a successful result to a new type.
    /// </summary>
    public async Task<Result<TNewValue>> MapAsync<TNewValue>(Func<TValue, Task<TNewValue>> mapper)
        where TNewValue : notnull
    {
        return _isSuccess 
            ? Result<TNewValue>.Success(await mapper(_value!)) 
            : Result<TNewValue>.Failure(_error!.Value);
    }

    /// <summary>
    /// Maps the error of a failed result to a new error type.
    /// </summary>
    public Result<TValue> MapError(Func<DomainError, DomainError> mapper)
    {
        return _isSuccess 
            ? this 
            : Result<TValue>.Failure(mapper(_error!.Value));
    }

    /// <summary>
    /// Maps both success and failure cases to new types.
    /// </summary>
    public Result<TNewValue> MapBoth<TNewValue>(
        Func<TValue, TNewValue> successMapper,
        Func<DomainError, DomainError> errorMapper)
        where TNewValue : notnull
    {
        return _isSuccess
            ? Result<TNewValue>.Success(successMapper(_value!))
            : Result<TNewValue>.Failure(errorMapper(_error!.Value));
    }

    /// <summary>
    /// Binds (flat maps) the result with a function that returns another Result.
    /// This is the core method for chaining operations that can fail.
    /// </summary>
    public Result<TNewValue> Bind<TNewValue>(Func<TValue, Result<TNewValue>> binder)
        where TNewValue : notnull
    {
        return _isSuccess ? binder(_value!) : Result<TNewValue>.Failure(_error!.Value);
    }

    /// <summary>
    /// Asynchronously binds the result with a function that returns another Result.
    /// </summary>
    public async Task<Result<TNewValue>> BindAsync<TNewValue>(Func<TValue, Task<Result<TNewValue>>> binder)
        where TNewValue : notnull
    {
        return _isSuccess ? await binder(_value!) : Result<TNewValue>.Failure(_error!.Value);
    }

    /// <summary>
    /// Applies a Result-wrapped function to a Result-wrapped value (Applicative pattern).
    /// </summary>
    public Result<TNewValue> Apply<TNewValue>(Result<Func<TValue, TNewValue>> functionResult)
        where TNewValue : notnull
    {
        return functionResult.IsSuccess && _isSuccess
            ? Result<TNewValue>.Success(functionResult.Value(_value!))
            : functionResult.IsFailure
                ? Result<TNewValue>.Failure(functionResult.Error)
                : Result<TNewValue>.Failure(_error!.Value);
    }

    /// <summary>
    /// Filters the result with a predicate. Returns failure if predicate is false.
    /// </summary>
    public Result<TValue> Filter(Func<TValue, bool> predicate, DomainError errorOnFalse)
    {
        return _isSuccess && predicate(_value!)
            ? this
            : _isSuccess
                ? Result<TValue>.Failure(errorOnFalse)
                : this;
    }

    /// <summary>
    /// Filters the result with a predicate. Returns failure if predicate is false.
    /// </summary>
    public Result<TValue> Filter(Func<TValue, bool> predicate, Func<TValue, DomainError> errorFactory)
    {
        return _isSuccess && predicate(_value!)
            ? this
            : _isSuccess
                ? Result<TValue>.Failure(errorFactory(_value!))
                : this;
    }

    /// <summary>
    /// Returns the current result if successful, otherwise returns the alternative result.
    /// </summary>
    public Result<TValue> Or(Result<TValue> alternative)
    {
        return _isSuccess ? this : alternative;
    }

    /// <summary>
    /// Returns the current result if successful, otherwise returns the result from the alternative function.
    /// </summary>
    public Result<TValue> Or(Func<DomainError, Result<TValue>> alternativeFactory)
    {
        return _isSuccess ? this : alternativeFactory(_error!.Value);
    }

    /// <summary>
    /// Recovers from a failure by providing a fallback value.
    /// </summary>
    public Result<TValue> Recover(TValue fallbackValue)
    {
        return _isSuccess ? this : Result<TValue>.Success(fallbackValue);
    }

    /// <summary>
    /// Recovers from a failure by using a function to provide a fallback value.
    /// </summary>
    public Result<TValue> Recover(Func<DomainError, TValue> fallbackFactory)
    {
        return _isSuccess ? this : Result<TValue>.Success(fallbackFactory(_error!.Value));
    }

    /// <summary>
    /// Recovers from a failure by using a function that returns a Result.
    /// </summary>
    public Result<TValue> RecoverWith(Func<DomainError, Result<TValue>> recoveryFactory)
    {
        return _isSuccess ? this : recoveryFactory(_error!.Value);
    }

    /// <summary>
    /// Taps into the success value without changing the result (for side effects).
    /// </summary>
    public Result<TValue> Tap(Action<TValue> action)
    {
        if (_isSuccess)
            action(_value!);
        return this;
    }

    /// <summary>
    /// Asynchronously taps into the success value without changing the result.
    /// </summary>
    public async Task<Result<TValue>> TapAsync(Func<TValue, Task> action)
    {
        if (_isSuccess)
            await action(_value!);
        return this;
    }

    /// <summary>
    /// Taps into the error without changing the result (for side effects).
    /// </summary>
    public Result<TValue> TapError(Action<DomainError> action)
    {
        if (!_isSuccess)
            action(_error!.Value);
        return this;
    }

    /// <summary>
    /// Asynchronously taps into the error without changing the result.
    /// </summary>
    public async Task<Result<TValue>> TapErrorAsync(Func<DomainError, Task> action)
    {
        if (!_isSuccess)
            await action(_error!.Value);
        return this;
    }

    #endregion

    #region Pattern Matching Methods

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

    #endregion

    #region Side Effect Methods (Backwards Compatibility)

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

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the value if successful, otherwise returns the default value.
    /// </summary>
    public TValue GetValueOrDefault(TValue defaultValue = default!)
    {
        return _isSuccess ? _value! : defaultValue;
    }

    /// <summary>
    /// Gets the value if successful, otherwise returns the result of the factory function.
    /// </summary>
    public TValue GetValueOrDefault(Func<DomainError, TValue> defaultFactory)
    {
        return _isSuccess ? _value! : defaultFactory(_error!.Value);
    }

    /// <summary>
    /// Throws an exception if the result is a failure.
    /// </summary>
    public TValue ThrowOnFailure()
    {
        return _isSuccess 
            ? _value! 
            : throw new InvalidOperationException($"Result failed with error: {_error!.Value.Message}");
    }

    /// <summary>
    /// Throws a custom exception if the result is a failure.
    /// </summary>
    public TValue ThrowOnFailure<TException>(Func<DomainError, TException> exceptionFactory)
        where TException : Exception
    {
        return _isSuccess 
            ? _value! 
            : throw exceptionFactory(_error!.Value);
    }

    #endregion

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
