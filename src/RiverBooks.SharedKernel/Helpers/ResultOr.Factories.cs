namespace RiverBooks.SharedKernel.Helpers;

public partial record ResultOr
{
    public static ResultOr Success() => new();

    public static ResultOr Failure(Error error) => new(error);

    public static ResultOr Failure(IEnumerable<Error> errors) => new(errors);

    public static ResultOr<TResult> Failure<TResult>(IEnumerable<Error> errors) => new(errors);

    public static ResultOr<TResult> Success<TResult>(TResult value) => new(value);
}
