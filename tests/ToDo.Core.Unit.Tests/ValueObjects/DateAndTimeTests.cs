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

    [Fact]
    public void Adding_Days_To_Valid_DateAndTime_Should_Succeed()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(6);
        var dateAndTime = new DateAndTime(date);
        var expectedDate = date.AddDays(1);
        
        // Act
        var result = dateAndTime.AddDays(1);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(expectedDate);
    }
}