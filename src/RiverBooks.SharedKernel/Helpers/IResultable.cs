namespace RiverBooks.SharedKernel.Helpers;

public interface IResultable
{
    bool IsSuccess { get; }

    List<Error>? Errors { get; }
}
