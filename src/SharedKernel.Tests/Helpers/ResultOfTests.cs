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
    public void ResulOfGuid_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(Guid.NewGuid());

    [Fact]
    public void ResulOfGuidEmpty_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(Guid.Empty);

    [Fact]
    public void ResulOfString_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure("!@#$%^&* !@#$%^ASDFd frgh");

    [Fact]
    public void ResulOfStringEmpty_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(string.Empty);

    [Fact]
    public void ResulOfStringNull_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure((string?)null);

    [Fact]
    public void ResulOfDecimal_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(decimal.MinValue);

    [Fact]
    public void ResulOfDecimalNull_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure((decimal?)null);

    [Fact]
    public void ResulOfArrayOfDecimal_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(new[] { decimal.MinValue, 0m, decimal.MaxValue });

    [Fact]
    public void ResulOfArrayOfDecimalNull_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure((decimal[]?)null);

    [Fact]
    public void ResulOfEnum_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(ApartmentState.STA);

    [Fact]
    public void ResulOfEnumNull_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure((ApartmentState?)null);

    [Fact]
    public void ResulOfObject_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(new SampleDto());

    [Fact]
    public void ResulOfObjectNull_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure((SampleDto?)null);

    [Fact]
    public void ResulOfObjectList_ShouldBeSerializable() =>
        CreateAndAssertSuccessAndFailure(new[]{ new SampleDto(), new SampleDto() { Id = 2, State = ApartmentState.Unknown}});

    private void CreateAndAssertSuccessAndFailure<T>(T value)
    {
        CreateAndAssertSuccess(value);
        CreateAndAssertFailure(value);
    }

    private void CreateAndAssertSuccess<T>(T value)
    {
        // Arrange
        var sut = ResultOf<T>.Success(value);

        // Assert sut
        sut.IsSuccess.Should().BeTrue();
        sut.Errors.Should().BeNull();

        // Act
        var json = JsonSerializer.Serialize(sut);
        var recreated = JsonSerializer.Deserialize<ResultOf<T>>(json);
        var recreatedJson = JsonSerializer.Serialize(recreated);

        // Assert recreated
        recreatedJson.Should().Be(json);
        recreated.IsSuccess.Should().BeTrue();
        recreated.Value.Should().BeEquivalentTo(sut.Value);
        recreated.Errors.Should().BeEquivalentTo(sut.Errors);
    }

    private void CreateAndAssertFailure<T>(T value)
    {
        // Arrange
        var sut = ResultOf<T>.Failure(new[] { Error.NullOrEmpty, Error.NotAuthorized });

        // Assert sut
        sut.IsSuccess.Should().BeFalse();
        sut.Errors.Should().NotBeNull();

        // Act
        var json = JsonSerializer.Serialize(sut);
        var recreated = JsonSerializer.Deserialize<ResultOf<T>>(json);
        var recreatedJson = JsonSerializer.Serialize(recreated);

        // Assert recreated
        recreatedJson.Should().Be(json);
        recreated.IsSuccess.Should().BeFalse();
        recreated.Value.Should().BeEquivalentTo(sut.Value);
        recreated.Errors.Should().BeEquivalentTo(sut.Errors);
    }
    
    [Fact]
    public void ResultOf_ShouldBeSerializable_WithFailure()
    {
        // Arrange
        var sut = ResultOf.Failure(new[] { Error.NullOrEmpty, Error.NotAuthorized });

        // Assert sut
        sut.IsSuccess.Should().BeFalse();
        sut.Errors.Should().NotBeNull();

        // Act
        var json = JsonSerializer.Serialize(sut);
        var recreated = JsonSerializer.Deserialize<ResultOf>(json);
        var recreatedJson = JsonSerializer.Serialize(recreated);

        // Assert recreated
        recreatedJson.Should().Be(json);
        recreated.IsSuccess.Should().BeFalse();
        recreated.Errors.Should().BeEquivalentTo(sut.Errors);
    }
    
    [Fact]
    public void ResultOf_ShouldBeSerializable_WithSuccess()
    {
        // Arrange
        var sut = ResultOf.Success();

        // Assert sut
        sut.IsSuccess.Should().BeTrue();
        sut.Errors.Should().BeNull();

        // Act
        var json = JsonSerializer.Serialize(sut);
        var recreated = JsonSerializer.Deserialize<ResultOf>(json);
        var recreatedJson = JsonSerializer.Serialize(recreated);

        // Assert recreated
        recreatedJson.Should().Be(json);
        recreated.IsSuccess.Should().BeTrue();
        recreated.Errors.Should().BeNull();
    }

    public class SampleDto
    {
        public int Id { get; set; } = 69;
        public string Name { get; set; } = "Sample name";
        public ApartmentState State { get; set; } = ApartmentState.STA;
        public int[] StateIds { get; set; } = Enumerable.Range(10, 10).ToArray();
    }
}

