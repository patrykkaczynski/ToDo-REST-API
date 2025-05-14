using Moq;
using Shouldly;
using ToDo.Application.Commands;
using ToDo.Application.Commands.Handlers;
using ToDo.Application.Exceptions;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Unit.Tests.Commands.Handlers;

public class MarkToDoTaskAsDoneHandlerTests
{
    [Fact]
    public async Task Handling_MarkToDoTaskAsDone_Command_With_Valid_ToDoTaskId_Should_Mark_As_Done()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var percentComplete = new PercentComplete(100);

        var toDoTask = ToDoTask.Create(toDoTaskId, new DateAndTime(Now.AddDays(2)),
            "Title", "Description", 50, new DateAndTime(Now));

        var command = new MarkToDoTaskAsDone(toDoTaskId);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync(toDoTask);

        var handler = new MarkToDoTaskAsDoneHandler(toDoTaskRepositoryMock.Object);

        // Act
        await handler.HandleAsync(command);

        // Assert
        toDoTaskRepositoryMock.Verify(r => r.UpdateAsync(It.Is<ToDoTask>(t =>
                t.Id == new ToDoTaskId(toDoTaskId) &&
                t.PercentComplete == percentComplete)),
            Times.Once);
    }

    [Fact]
    public async Task
        Handling_MarkToDoTaskAsDone_Command_With_Nonexistent_ToDoTaskId_Should_Throw_ToDoTaskNotFoundException()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        var command = new MarkToDoTaskAsDone(toDoTaskId);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync((ToDoTask)null);

        var handler = new MarkToDoTaskAsDoneHandler(toDoTaskRepositoryMock.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<ToDoTaskNotFoundException>();
    }

    #region ARRANGE

    private static readonly DateTime Now = new DateTime(2025, 04, 28);

    #endregion
}