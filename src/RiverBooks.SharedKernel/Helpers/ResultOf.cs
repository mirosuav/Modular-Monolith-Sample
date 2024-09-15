using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// Optional <see cref="ResultOf"/>. Is either successful or 'Error'
/// </summary>
public readonly struct ResultOf : IResultOf
{
    private readonly IList<Error>? _errors;

    [MemberNotNullWhen(false, nameof(Errors))]
    public bool IsSuccess { get; }

    public IReadOnlyList<Error>? Errors => _errors?.AsReadOnly();

    [JsonConstructor]
    private ResultOf(bool isSuccess, IReadOnlyList<Error>? errors)
    {
        IsSuccess = isSuccess;
        _errors = errors?.ToList() ?? default;
    }

    public ResultOf(Error error) =>
        (IsSuccess, _errors) = (false, new List<Error> { error });

    public ResultOf(IEnumerable<Error> errors) =>
        (IsSuccess, _errors) = (false, errors.ToList());

    public TMatchedResult Match<TMatchedResult>(
        Func<TMatchedResult> successProcessor,
        Func<IReadOnlyList<Error>, TMatchedResult> errorProcessor) =>
        IsSuccess
            ? successProcessor()
            : errorProcessor(Errors);

    public Task<TMatchedResult> MatchAsync<TMatchedResult>(
        Func<Task<TMatchedResult>> successProcessor,
        Func<IReadOnlyList<Error>, Task<TMatchedResult>> errorProcessor) =>
        IsSuccess
            ? successProcessor()
            : errorProcessor(Errors);

    public void Switch(
        Action onSuccess,
        Action<IReadOnlyList<Error>> onError)
    {
        if (IsSuccess)
            onSuccess();
        else
            onError(Errors);
    }

    public Task SwitchAsync(
        Func<Task> onSuccessAsync,
        Func<IReadOnlyList<Error>, Task> onErrorAsync) =>
        IsSuccess
            ? onSuccessAsync()
            : onErrorAsync(Errors);

    public static ResultOf Success() =>
        new(true, null);

    public static ResultOf Failure(Error error) =>
        new(error);

    public static ResultOf Failure(IEnumerable<Error> errors) =>
        new(errors);

    /// <summary>
    /// Implicit conversion from bool only accepts conversion of 'true' to success result
    /// </summary>
    /// <param name="value">Boolean true value only</param>
    public static implicit operator ResultOf(bool value) =>
        value
            ? new(true, null)
            : throw new InvalidOperationException($"Invalid {nameof(ResultOf)} value. Did you intended to return explicit {nameof(Error)} instance ?");

    public static implicit operator ResultOf(Error error) =>
        new(error);
}
