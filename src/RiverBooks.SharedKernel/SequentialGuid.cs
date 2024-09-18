using RiverBooks.SharedKernel.Extensions;

namespace RiverBooks.SharedKernel;

public static class SequentialGuid
{
    public static Guid Empty = Guid.Empty;

    /// <summary>
    ///     Generate new sequential Guid v7
    ///     <see cref="Uuid7" />
    /// </summary>
    public static Guid NewGuid()
    {
        return Uuid7.Guid();
    }

    /// <summary>
    ///     Generate new human readable sequential identifier like 0q994uri70qe0gjxrq8iv4iyu
    ///     <see cref="Uuid7" />
    /// </summary>
    public static string NewHumanReadableId()
    {
        return Uuid7.Id25();
    }
}