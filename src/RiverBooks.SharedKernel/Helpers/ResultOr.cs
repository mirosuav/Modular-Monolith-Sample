using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RiverBooks.SharedKernel.Helpers;

/// <summary>
/// Optional Result Is either sucessfull or Error
/// </summary>
public partial record ResultOr : IResultOr
{
    [JsonInclude]
    [MemberNotNullWhen(false, nameof(Errors))]
    public virtual bool IsSuccess { get; }

    [JsonInclude]
    public virtual List<Error>? Errors { get; }

    [JsonIgnore]
    public virtual Error Error => Errors?.FirstOrDefault() ?? Error.None;

    public ResultOr() =>
        IsSuccess = true;

    public ResultOr(Error error) =>
        (IsSuccess, Errors) = (false, [error]);

    public ResultOr(IEnumerable<Error> errors) =>
        (IsSuccess, Errors) = (false, errors is List<Error> list ? list : errors.ToList());

    public TMatchedResult Match<TMatchedResult>(
        Func<TMatchedResult> successProcessor,
        Func<List<Error>, TMatchedResult> errorProcessor) =>
        IsSuccess ? successProcessor() : errorProcessor(Errors);

    public Task<TMatchedResult> MatchAsync<TMatchedResult>(
        Func<Task<TMatchedResult>> successProcessor,
        Func<List<Error>, Task<TMatchedResult>> errorProcessor) =>
        IsSuccess ? successProcessor() : errorProcessor(Errors);

    public void Switch(Action onSuccessDo, Action onErrorDo)
    {
        if (IsSuccess)
            onSuccessDo();
        else
            onErrorDo();
    }

    public Task SwitchAsync(Func<Task> onSuccessDoAsync, Func<Task> onErrorDoAsync) =>
        IsSuccess ? onSuccessDoAsync() : onErrorDoAsync();

    public override string ToString() =>
        IsSuccess ? "Success" : Error!.ToString();

    public static implicit operator ResultOr(Error error) =>
        new(error);

    public static implicit operator ResultOr(List<Error> errors) =>
        Failure(errors);
}
