using Shouldly;
using ToDo.Core.Entities;
using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.Entities;

public class ToDoTaskTests
{
    [Theory]
    [MemberData(nameof(IncorrectExpirationDates))]
    public void Creating_ToDoTask_With_ExpirationDate_In_The_Past_Or_Now_Should_Throw_InvalidExpirationDateException(
        DateTime expiredAt)
    {
        // Arrange
        var expirationDate = new DateAndTime(expiredAt);
        var now = new DateAndTime(Now);

        // Act
        var exception = Record.Exception(() => ToDoTask.Create(Guid.NewGuid(), expirationDate,
            "Title", "Description", 50, now));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidExpirationDateException>();
    }

    [Theory]
    [MemberData(nameof(CorrectExpirationDates))]
    public void Creating_ToDoTask_With_Correct_ExpirationDate_Should_Succeed(DateTime expiredAt)
    {
        // Arrange
        var toDoTaskId = new ToDoTaskId(Guid.NewGuid());
        var expirationDate = new DateAndTime(expiredAt);
        var title = new Title("Title");
        var description = new Description("Description");
        var percentComplete = new PercentComplete(50);
        var now = new DateAndTime(Now);

        // Act
        var toDoTask = ToDoTask.Create(toDoTaskId, expirationDate,
            title, description, percentComplete, now);

        // Assert
        toDoTask.ShouldNotBeNull();
        toDoTask.Id.ShouldBe(toDoTaskId);
        toDoTask.Title.ShouldBe(title);
        toDoTask.Description.ShouldBe(description);
        toDoTask.PercentComplete.ShouldBe(percentComplete);
        toDoTask.ExpirationDate.ShouldBe(expirationDate);
    }

    [Theory]
    [MemberData(nameof(IncorrectExpirationDates))]
    public void Updating_ToDoTask_With_ExpirationDate_In_The_Past_Or_Now_Should_Throw_InvalidExpirationDateException(
        DateTime expiredAt)
    {
        // Arrange
        var expirationDate = new DateAndTime(Now.AddDays(1));
        var now = new DateAndTime(Now);
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), expirationDate, "Title",
            "Description", 50, now);

        // Act
        var exception = Record.Exception(() => toDoTask.Update(new DateAndTime(expiredAt),
            "Updated Title", "Updated Description", 80, now));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidExpirationDateException>();
    }

    [Theory]
    [MemberData(nameof(CorrectExpirationDates))]
    public void Updating_ToDoTask_With_Correct_ExpirationDate_Should_Succeed(DateTime expiredAt)
    {
        // Arrange
        var expirationDate = new DateAndTime(expiredAt.AddDays(1));
        var title = new Title("Updated Title");
        var description = new Description("Updated Description");
        var percentComplete = new PercentComplete(70);
        var now = new DateAndTime(Now);

        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(expiredAt),"Title", 
            "Description", new PercentComplete(50), now);

        // Act
        toDoTask.Update(expirationDate, title, description, percentComplete, now);

        // Assert
        toDoTask.ShouldNotBeNull();
        toDoTask.Title.ShouldBe(title);
        toDoTask.Description.ShouldBe(description);
        toDoTask.PercentComplete.ShouldBe(percentComplete);
        toDoTask.ExpirationDate.ShouldBe(expirationDate);
    }

    [Fact]
    public void Updating_PercentComplete_Should_Succeed()
    {
        // Arrange
        var percentComplete = new PercentComplete(80);
        var toDoTask = ToDoTask.Create(Guid.NewGuid(),  new DateAndTime(Now.AddDays(1)),"Title", 
            "Description", new PercentComplete(50),   new DateAndTime(Now)); 
        
        // Act
        toDoTask.ChangePercentComplete(percentComplete);
        
        // Assert
        toDoTask.PercentComplete.ShouldBe(percentComplete);
    }

    [Fact]
    public void Marking_ToDoTask_As_Done_Should_Succeed()
    {
        // Arrange
        var percentComplete = new PercentComplete(100);
        var toDoTask = ToDoTask.Create(Guid.NewGuid(),  new DateAndTime(Now.AddDays(1)),"Title", 
            "Description", new PercentComplete(50),   new DateAndTime(Now)); 
        
        // Act
        toDoTask.ChangePercentComplete(100);

        // Assert
        toDoTask.PercentComplete.ShouldBe(percentComplete);
    }
    
    #region ARRANGE

    private static readonly DateTime Now = new DateTime(2025, 04, 28);

    public static IEnumerable<object[]> IncorrectExpirationDates =>
        new List<object[]>
        {
            new object[] { Now },
            new object[] { Now.AddDays(-1) },
            new object[] { Now.AddMinutes(-1) },
            new object[] { Now.AddSeconds(-1) },
        };

    public static IEnumerable<object[]> CorrectExpirationDates =>
        new List<object[]>
        {
            new object[] { Now.AddDays(1) },
            new object[] { Now.AddMinutes(1) },
            new object[] { Now.AddSeconds(1) },
        };

    #endregion
}