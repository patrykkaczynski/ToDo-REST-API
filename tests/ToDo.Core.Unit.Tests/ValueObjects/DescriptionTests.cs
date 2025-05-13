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
    public void Creating_Description_With_Empty_Value_Should_Throw_EmptyDescriptionException(string description)
    {
        // Act
        var exception = Record.Exception(() => new Description(description));
        
        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<EmptyDescriptionException>();
    }
    
    [Fact]
    public void Creating_Description_With_Length_Greater_Than_500_Characters_Should_Throw_InvalidDescriptionException()
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
    public void Creating_Valid_Description_Should_Succeed()
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