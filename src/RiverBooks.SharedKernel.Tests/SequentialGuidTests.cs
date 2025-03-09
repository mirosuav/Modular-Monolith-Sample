using FluentAssertions;

namespace RiverBooks.SharedKernel.Tests;

public class SequentialGuidTests
{
    [Fact]
    public void NewGuid_ShouldReturnSortedGuids()
    {
        // ACT
        var guids = Enumerable
            .Range(1, 10_000_000).Select(_ => SequentialGuid.NewGuid())
            .ToList();

        // ASSERT
        guids.Should().OnlyHaveUniqueItems();
        guids.Should().BeInAscendingOrder();
    }

    [Fact]
    public void NewGuid_ShouldCauseNoCollisions()
    {
        // ARRANGE
        var size = 10_000_000;
        var guids = new Guid[size];

        // ACT
        Parallel.For(0, size - 1, i => guids[i] = SequentialGuid.NewGuid());

        // ASSERT
        Array.Sort(guids);
        guids.Should().OnlyHaveUniqueItems();
    }
}