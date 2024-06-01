namespace RiverBooks.SharedKernel.Helpers;

public partial record Resultable
{
    public static Resultable Success() => new();

    public static Resultable Failure(Error error) => new(error);

    public static Resultable Failure(IEnumerable<Error> errors) => new(errors);

    public static Resultable<TResult> Failure<TResult>(IEnumerable<Error> errors) => new(errors);

    public static Resultable<TResult> Success<TResult>(TResult value) => new(value);
}
