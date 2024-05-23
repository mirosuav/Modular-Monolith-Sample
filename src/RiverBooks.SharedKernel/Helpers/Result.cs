using System.Text.Json;

namespace RiverBooks.SharedKernel.Helpers;

public interface IResult
{
    bool IsFaulted { get; }
}

/// <summary>
/// Optional Result of type T. Is either sucessfull 'Value' object of type T or 'Error'
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly record struct Result<T> : IResult
{
    public readonly bool IsSuccess;
    public readonly T? Value;
    public readonly List<Error>? Errors;

    public Error Error => Errors?.FirstOrDefault() ?? Error.None;

    public bool IsFaulted => !IsSuccess;

    public Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    public Result(Error error)
    {
        IsSuccess = false;
        Value = default;
        Errors = [error];
    }

    public Result(IEnumerable<Error> errors)
    {
        IsSuccess = false;
        Value = default;
        Errors = errors is List<Error> list ? list : errors.ToList();
    }

    public TResult Match<TResult>(
        Func<T, TResult> successProcessor,
        Func<IReadOnlyList<Error>, TResult> errorProcessor)
    {
        return IsSuccess ? successProcessor(Value!) : errorProcessor(Errors!);
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> successProcessor,
        Func<IReadOnlyList<Error>, Task<TResult>> errorProcessor)
    {
        return IsSuccess
            ? await successProcessor(Value!).ConfigureAwait(false)
            : await errorProcessor(Errors!).ConfigureAwait(false);
    }

    public override string ToString()
        => IsSuccess
        ? Value?.ToString() ?? string.Empty
        : Error!.ToString();

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);

    public static implicit operator Result<T>(List<Error> errors) => Failure(errors);

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);

    public static Result<T> Failure(IEnumerable<Error>? errors) => new(errors ?? new List<Error>());

    public string AsJson()
        => IsSuccess
        ? JsonSerializer.Serialize(Value)
        : JsonSerializer.Serialize(Errors);
}
