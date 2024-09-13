namespace RiverBooks.SharedKernel.Helpers;

public interface IResultable
{
    bool IsSuccess { get; }

    IReadOnlyList<Error>? Errors { get; }
}
