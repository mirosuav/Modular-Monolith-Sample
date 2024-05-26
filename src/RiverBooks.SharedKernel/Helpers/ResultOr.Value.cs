using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// Optional ResultOr of type T. Is either sucessfull 'Value' object of type T or 'Error'
/// </summary>
/// <typeparam name="TResult"></typeparam>
public record ResultOr<TResult> : ResultOr
{
    [JsonInclude]
    public readonly TResult? Value;

    [MemberNotNullWhen(true, nameof(Value))]
    public override bool IsSuccess => base.IsSuccess;

    public ResultOr(TResult value) : base() =>
        Value = value;

    public ResultOr(Error error) : base(error) =>
        Value = default;

    public ResultOr(IEnumerable<Error> errors) : base(errors) =>
        Value = default;

    public ResultOr<TMppedResult> Map<TMppedResult>(Func<TResult, TMppedResult> successMapper) =>
        IsSuccess ? successMapper(Value) : Errors;

    public TMatchedResult Match<TMatchedResult>(
        Func<TResult, TMatchedResult> successProcessor,
        Func<List<Error>, TMatchedResult> errorProcessor) =>
        IsSuccess ? successProcessor(Value) : errorProcessor(Errors);

    public Task<TMatchedResult> MatchAsync<TMatchedResult>(
        Func<TResult, Task<TMatchedResult>> successProcessor,
        Func<List<Error>, Task<TMatchedResult>> errorProcessor) =>
        IsSuccess ? successProcessor(Value) : errorProcessor(Errors);

    public override string ToString()
        => IsSuccess
        ? Value?.ToString() ?? string.Empty
        : Error!.ToString();

    public static implicit operator ResultOr<TResult>(TResult value) =>
        new(value);

    public static implicit operator ResultOr<TResult>(Error error) =>
        new(error);

    public static implicit operator ResultOr<TResult>(List<Error> errors) =>
        Failure<TResult>(errors);

    public string AsJson()
        => IsSuccess
        ? JsonSerializer.Serialize(Value)
        : JsonSerializer.Serialize(Errors);
}
