using FluentAssertions;

namespace RiverBooks.SharedKernel.Tests;

public class SequentialGuidTests
{
    [Fact]
    public void NewGuid_ShouldReturnSortedGuids()
    {
        // ACT
        var guids = Enumerable
            .Range(1, 1_000_000).Select(_ => SequentialGuid.NewGuid())
            .Select(g => g.ToString())
            .ToList();

        // ASSERT
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