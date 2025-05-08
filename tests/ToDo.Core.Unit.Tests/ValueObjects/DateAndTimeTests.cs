using Shouldly;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.ValueObjects;

public class DateAndTimeTests
{
    [Fact]
    public void Creating_Correct_ExpirationDate_Should_Succeed()
    {
        // Arrange
        var dateAndTime = DateTime.UtcNow.AddDays(6);
        
        // Act
        var result = new DateAndTime(dateAndTime);
        
        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(dateAndTime);
    }
}