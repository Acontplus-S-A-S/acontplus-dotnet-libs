using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Acontplus.Core.Domain.Common;

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public readonly record struct Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; }

    [JsonPropertyName("value")]
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value of a failed result");

    [JsonPropertyName("error")]
    public TError Error => !IsSuccess ? _error! : throw new InvalidOperationException("Cannot access Error of a successful result");

    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    private Result(TValue value)
    {
        _value = value;
        _error = default;
        IsSuccess = true;
    }

    private Result(TError error)
    {
        _value = default;
        _error = error;
        IsSuccess = false;
    }

    public static Result<TValue, TError> Success(TValue value) => new(value);
    public static Result<TValue, TError> Failure(TError error) => new(error);

    public static implicit operator Result<TValue, TError>(TValue value) => Success(value);
    public static implicit operator Result<TValue, TError>(TError error) => Failure(error);

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(_error!);

    public void Match(Action<TValue> onSuccess, Action<TError> onFailure)
    {
        if (IsSuccess) onSuccess(_value!);
        else onFailure(_error!);
    }

    public Result<TResult, TError> Map<TResult>(Func<TValue, TResult> mapper) =>
        IsSuccess ? Result<TResult, TError>.Success(mapper(_value!)) : Result<TResult, TError>.Failure(_error!);

    public async Task<Result<TResult, TError>> MapAsync<TResult>(Func<TValue, Task<TResult>> mapper) =>
        IsSuccess ? Result<TResult, TError>.Success(await mapper(_value!)) : Result<TResult, TError>.Failure(_error!);

    public Result<TResult, TError> Bind<TResult>(Func<TValue, Result<TResult, TError>> binder) =>
        IsSuccess ? binder(_value!) : Result<TResult, TError>.Failure(_error!);

    public async Task<Result<TResult, TError>> BindAsync<TResult>(Func<TValue, Task<Result<TResult, TError>>> binder) =>
        IsSuccess ? await binder(_value!) : Result<TResult, TError>.Failure(_error!);

    public bool TryGetValue([NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out TError? error)
    {
        if (IsSuccess)
        {
            value = _value!;
            error = default;
            return true;
        }

        value = default;
        error = _error!;
        return false;
    }
}

public static class Result
{
    public static Result<TValue, DomainError> Success<TValue>(TValue value) =>
        Result<TValue, DomainError>.Success(value);

    public static Result<TValue, DomainError> Failure<TValue>(DomainError error) =>
        Result<TValue, DomainError>.Failure(error);

    public static Result<SuccessWithWarnings<TValue>, DomainError> SuccessWithWarnings<TValue>(
        TValue value, DomainWarnings warnings) =>
        Result<SuccessWithWarnings<TValue>, DomainError>.Success(new SuccessWithWarnings<TValue>(value, warnings));

    // Renamed for clarity when dealing with multiple errors
    public static Result<TValue, DomainErrors> SuccessWithMultipleErrors<TValue>(TValue value) where TValue : class =>
        Result<TValue, DomainErrors>.Success(value);

    public static Result<TValue, DomainErrors> Failure<TValue>(DomainErrors errors) where TValue : class =>
        Result<TValue, DomainErrors>.Failure(errors);
}