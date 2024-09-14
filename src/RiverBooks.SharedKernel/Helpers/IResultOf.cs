namespace RiverBooks.SharedKernel.Helpers;

public interface IResultOf
{
    bool IsSuccess { get; }

    IReadOnlyList<Error>? Errors { get; }
}
