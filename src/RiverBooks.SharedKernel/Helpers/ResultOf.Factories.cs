namespace RiverBooks.SharedKernel.Helpers;

public readonly partial struct ResultOf<T>
{
    public static ResultOf<T> Success(T value) => new(value);

    public static ResultOf<T> Failure(Error error) => new(error);

    public static ResultOf<T> Failure(IEnumerable<Error> errors) => new(errors);
}
