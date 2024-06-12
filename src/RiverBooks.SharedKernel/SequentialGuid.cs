using UuidExtensions;

namespace RiverBooks.SharedKernel;

public static class SequentialGuid
{
    /// <summary>
    /// Generate new sequential Guid v7
    /// <see cref="Uuid7"/>
    /// </summary>
    public static Guid NewGuid() => Uuid7.Guid();

    public static Guid Empty = Guid.Empty;

    /// <summary>
    /// Generate new human readable sequential identifier like 0q994uri70qe0gjxrq8iv4iyu
    /// <see cref="Uuid7"/>
    /// </summary>
    public static string NewHumanReadableId() => Uuid7.Id25();
}
