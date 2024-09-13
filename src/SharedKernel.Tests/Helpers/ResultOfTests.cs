using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.SharedKernel.Tests.Helpers;

public class ResultOfTests
{
    [Fact]
    public void Resultable_ShouldBeSerializable()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var sut = ResultOf<Guid>.Success(id);
        var json = JsonSerializer.Serialize(sut);
        var recreated = JsonSerializer.Deserialize<ResultOf<Guid>>(json);

        // Assert
        recreated.Should().Be(sut);
    }
}

