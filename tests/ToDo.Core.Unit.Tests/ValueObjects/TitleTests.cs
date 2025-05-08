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
    public void Creating_Title_With_Empty_Value_Should_Throw_EmptyTitleException(string title)
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
    public void Creating_Title_With_Length_Less_Than_3_Or_Greater_Than_50_Characters_Should_Throw_InvalidTitleException(string title)
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
    public void Creating_Correct_Title_Should_Succeed(string title)
    {
        // Act
        var result = new Title(title);
        
        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(title);
    }
}