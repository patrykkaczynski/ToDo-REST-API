using Shouldly;
using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Unit.Tests.ValueObjects;

public class ToDoTaskIdTests
{
    [Fact]
    public void Creating_ToDoTaskId_With_Empty_Guid_Should_Throw_InvalidEntityIdException()
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
    public void Creating_Correct_ToDoTaskId_Should_Succeed()
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