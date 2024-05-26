namespace RiverBooks.SharedKernel.Helpers;

public interface IResultOr
{
    bool IsSuccess { get; }

    List<Error>? Errors { get; }
}
