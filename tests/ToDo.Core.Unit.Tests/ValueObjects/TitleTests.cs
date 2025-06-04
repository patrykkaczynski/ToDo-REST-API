using Shouldly;
using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.ValueObjects;

public class TitleTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WhenTitleIsNullOrWhitespace_ShouldThrowEmptyTitleException(string title)
    {
        // Act
        var exception = Record.Exception(() => new Title(title));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<EmptyTitleException>();
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a very very very very very very very very very very long title")]
    public void Constructor_WhenTitleLengthIsLessThan3OrGreaterThan50_ShouldThrowInvalidTitleException(string title)
    {
        // Act
        var exception = Record.Exception(() => new Title(title));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidTitleException>();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("This is a description with exactly fifty chars.")]
    public void Constructor_WhenTitleIsValid_ShouldCreateTitleInstance(string title)
    {
        // Act
        var result = new Title(title);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(title);
    }
}