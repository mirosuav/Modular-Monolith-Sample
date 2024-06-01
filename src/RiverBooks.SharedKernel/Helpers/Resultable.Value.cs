using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// Optional <see cref="Resultable"/> of type T. Is either sucessfull 'Value' object of type T or 'Error'
/// </summary>
/// <typeparam name="TResult"></typeparam>
public record Resultable<TResult> : Resultable
{
    [JsonInclude]
    public readonly TResult? Value;

    [MemberNotNullWhen(true, nameof(Value))]
    public override bool IsSuccess => base.IsSuccess;

    public Resultable(TResult value) : base() =>
        Value = value;

    public Resultable(Error error) : base(error) =>
        Value = default;

    public Resultable(IEnumerable<Error> errors) : base(errors) =>
        Value = default;

    public Resultable<TMppedResult> Map<TMppedResult>(Func<TResult, TMppedResult> successMapper) =>
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

    public static implicit operator Resultable<TResult>(TResult value) =>
        new(value);

    public static implicit operator Resultable<TResult>(Error error) =>
        new(error);

    public static implicit operator Resultable<TResult>(List<Error> errors) =>
        Failure<TResult>(errors);

    public string AsJson()
        => IsSuccess
        ? JsonSerializer.Serialize(Value)
        : JsonSerializer.Serialize(Errors);
}
