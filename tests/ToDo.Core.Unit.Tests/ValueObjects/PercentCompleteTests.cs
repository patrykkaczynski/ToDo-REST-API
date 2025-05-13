using Shouldly;
using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.ValueObjects;

public class PercentCompleteTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Creating_PercentComplete_With_Value_Less_Than_Zero_Or_Greater_Than_100_Should_Throw_InvalidPercentCompleteException
        (int percentComplete)
    {
        // Act
        var exception = Record.Exception(() => new PercentComplete(percentComplete));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidPercentCompleteException>();
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Creating_Valid_PercentComplete_Should_Succeed(int percentComplete)
    {
        // Act
        var result = new PercentComplete(percentComplete);
        
        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(percentComplete);
    }
}