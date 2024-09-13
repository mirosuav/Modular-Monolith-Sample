using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// Optional <see cref="ResultOf{T}"/> of type T. Is either successful 'Value' object of type T or 'Error'
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly partial struct ResultOf<T> : IResultable
{
    [JsonInclude]
    public readonly T? Value;

    [JsonInclude]
    private readonly IList<Error>? _errors;

    [JsonInclude]
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Errors))]
    public bool IsSuccess { get; }

    public IReadOnlyList<Error>? Errors => _errors?.AsReadOnly();

    public ResultOf(T value) =>
        (IsSuccess, Value) = (true, value);

    public ResultOf(Error error) =>
        (IsSuccess, _errors) = (false, new List<Error> { error });

    public ResultOf(IEnumerable<Error> errors) =>
        (IsSuccess, _errors) = (false, errors.ToList());

    public static implicit operator ResultOf<T>(T value) =>
        new(value);

    public static implicit operator ResultOf<T>(Error error) =>
        new(error);

    public override string ToString()
        => IsSuccess
            ? Value.ToString() ?? string.Empty
            : JsonSerializer.Serialize(Errors);

    public string AsJson()
        => IsSuccess
        ? JsonSerializer.Serialize(Value)
        : JsonSerializer.Serialize(Errors);
}
