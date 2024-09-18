using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
///     Optional <see cref="ResultOf{T}" /> of type T. Is either successful 'Value' object of type T or 'Error'
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerStepThrough]
public readonly record struct ResultOf<T> : IResultOf
{
    private readonly IList<Error>? _errors;

    [JsonConstructor]
    private ResultOf(bool isSuccess, T? value, IReadOnlyList<Error>? errors)
    {
        IsSuccess = isSuccess;
        Value = value ?? default;
        _errors = errors?.ToList() ?? default;
    }

    public ResultOf(T value)
    {
        (IsSuccess, Value) = (true, value);
    }

    public ResultOf(Error error)
    {
        (IsSuccess, _errors) = (false, new List<Error> { error });
    }

    public ResultOf(IEnumerable<Error> errors)
    {
        (IsSuccess, _errors) = (false, errors.ToList());
    }

    public T? Value { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Errors))]
    public bool IsSuccess { get; }

    public IReadOnlyList<Error>? Errors => _errors?.AsReadOnly();

    public override string ToString()
    {
        return AsJson();
    }

    public string AsJson()
    {
        return IsSuccess
            ? JsonSerializer.Serialize(Value)
            : JsonSerializer.Serialize(Errors);
    }

    public TMatchedResult Match<TMatchedResult>(
        Func<T, TMatchedResult> successProcessor,
        Func<IReadOnlyList<Error>, TMatchedResult> errorProcessor)
    {
        return IsSuccess
            ? successProcessor(Value)
            : errorProcessor(Errors);
    }

    public Task<TMatchedResult> MatchAsync<TMatchedResult>(
        Func<T, Task<TMatchedResult>> successProcessor,
        Func<IReadOnlyList<Error>, Task<TMatchedResult>> errorProcessor)
    {
        return IsSuccess
            ? successProcessor(Value)
            : errorProcessor(Errors);
    }

    public ResultOf<TNew> Map<TNew>(
        Func<T, TNew> valueMapper)
    {
        return IsSuccess
            ? new ResultOf<TNew>(valueMapper(Value))
            : new ResultOf<TNew>(Errors);
    }

    public void Switch(
        Action<T> onSuccess,
        Action<IReadOnlyList<Error>> onError)
    {
        if (IsSuccess)
            onSuccess(Value);
        else
            onError(Errors);
    }

    public Task SwitchAsync(
        Func<T, Task> onSuccessAsync,
        Func<IReadOnlyList<Error>, Task> onErrorAsync)
    {
        return IsSuccess
            ? onSuccessAsync(Value)
            : onErrorAsync(Errors);
    }

    public static ResultOf<T> Success(T value)
    {
        return new ResultOf<T>(value);
    }

    public static ResultOf<T> Failure(Error error)
    {
        return new ResultOf<T>(error);
    }

    public static ResultOf<T> Failure(IEnumerable<Error> errors)
    {
        return new ResultOf<T>(errors);
    }

    public static implicit operator ResultOf<T>(T value)
    {
        return new ResultOf<T>(value);
    }

    public static implicit operator ResultOf<T>(Error error)
    {
        return new ResultOf<T>(error);
    }
}