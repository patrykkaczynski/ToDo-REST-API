using Shouldly;
using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.ValueObjects;

public class ToDoTaskIdTests
{
    [Fact]
    public void Constructor_WhenGuidIsEmpty_ShouldThrowInvalidEntityIdException()
    {
        // Arrange
        var toDoTaskId = Guid.Empty;

        // Act
        var exception = Record.Exception(() => new ToDoTaskId(toDoTaskId));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<InvalidEntityIdException>();
    }

    [Fact]
    public void Constructor_WhenGuidIsValid_ShouldCreateToDoTaskIdInstance()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        // Act
        var result = new ToDoTaskId(toDoTaskId);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe(toDoTaskId);
    }
}