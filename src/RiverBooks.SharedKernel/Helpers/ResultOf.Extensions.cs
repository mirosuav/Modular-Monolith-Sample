namespace RiverBooks.SharedKernel.Helpers;
public readonly partial struct ResultOf<T> : IResultOf
{
    public TMatchedResult Match<TMatchedResult>(
        Func<T, TMatchedResult> successProcessor,
        Func<IReadOnlyList<Error>, TMatchedResult> errorProcessor) =>
        IsSuccess
            ? successProcessor(Value)
            : errorProcessor(Errors);

    public Task<TMatchedResult> MatchAsync<TMatchedResult>(
        Func<T, Task<TMatchedResult>> successProcessor,
        Func<IReadOnlyList<Error>, Task<TMatchedResult>> errorProcessor) =>
        IsSuccess
            ? successProcessor(Value)
            : errorProcessor(Errors);

    public ResultOf<TNew> Map<TNew>(
        Func<T, TNew> valueMapper) =>
        IsSuccess
            ? new ResultOf<TNew>(valueMapper(Value))
            : new ResultOf<TNew>(Errors);

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
        Func<IReadOnlyList<Error>, Task> onErrorAsync) =>
        IsSuccess
            ? onSuccessAsync(Value)
            : onErrorAsync(Errors);
}

