using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// Optional <see cref="ResultOf{T}"/> of type T. Is either successful 'Value' object of type T or 'Error'
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly partial struct ResultOf<T> : IResultOf
{
    private readonly IList<Error>? _errors;
    
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Errors))]
    public bool IsSuccess { get; }

    public T? Value { get; }

    public IReadOnlyList<Error>? Errors => _errors?.AsReadOnly();

    [JsonConstructor]
    private ResultOf(bool isSuccess, T? value, IReadOnlyList<Error>? errors)
    {
        IsSuccess = isSuccess;
        Value = value ?? default;
        _errors = errors?.ToList() ?? default;
    }

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

    public string AsJson()
        => IsSuccess
        ? JsonSerializer.Serialize(Value)
        : JsonSerializer.Serialize(Errors);
}
