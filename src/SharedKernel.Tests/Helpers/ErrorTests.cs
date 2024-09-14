using FluentAssertions;
using RiverBooks.SharedKernel.Helpers;
using System.Text.Json;

namespace RiverBooks.SharedKernel.Tests.Helpers;

public class ErrorTests
{
    [Fact]
    public void Error_ShouldBeSerializable_NullOrEmpty()
    {
        var sut = Error.NullOrEmpty;

        var json = JsonSerializer.Serialize(sut);

        var recreated = JsonSerializer.Deserialize<Error>(json);

        recreated.Should().Be(sut);
    }

    [Fact]
    public void Error_ShouldBeSerializable_Unauthorized()
    {
        var sut = Error.Unauthorized("User is not authorized.");

        var json = JsonSerializer.Serialize(sut);

        var recreated = JsonSerializer.Deserialize<Error>(json);

        recreated.Should().Be(sut);
    }

}

