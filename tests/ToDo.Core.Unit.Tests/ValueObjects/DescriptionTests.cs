using Shouldly;
using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.ValueObjects;

public class DescriptionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WhenDescriptionIsNullOrWhitespace_ShouldThrowEmptyDescriptionException(string description)
    {
        // Act
        var exception = Record.Exception(() => new Description(description));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<EmptyDescriptionException>();
    }

    [Fact]
    public void Constructor_WhenDescriptionLengthExceeds500_ShouldThrowInvalidDescriptionException()
    {
        // Arrange
        var description = new string('a', 501);

        // Act
        var exception = Record.Exception(() => new Description(description));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidDescriptionException>();
    }

    [Fact]
    public void Constructor_WhenDescriptionIsValid_ShouldCreateDescriptionInstance()
    {
        // Arrange
        var description = new string('a', 500);

        // Act
        var result = new Description(description);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(description);
    }
}