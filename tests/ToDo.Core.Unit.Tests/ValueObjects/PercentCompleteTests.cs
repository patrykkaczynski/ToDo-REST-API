using Shouldly;
using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.ValueObjects;

public class PercentCompleteTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_WhenValueIsLessThanZeroOrGreaterThanHundred_ShouldThrowInvalidPercentCompleteException
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
    public void Constructor_WhenValueIsBetweenZeroAndHundredInclusive_ShouldCreatePercentCompleteInstance
        (int percentComplete)
    {
        // Act
        var result = new PercentComplete(percentComplete);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(percentComplete);
    }
}