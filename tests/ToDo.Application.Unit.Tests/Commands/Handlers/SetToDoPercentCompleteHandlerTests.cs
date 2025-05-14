using Moq;
using Shouldly;
using ToDo.Application.Commands;
using ToDo.Application.Commands.Handlers;
using ToDo.Application.Exceptions;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Unit.Tests.Commands.Handlers;

public class SetToDoPercentCompleteHandlerTests
{
    [Fact]
    public async Task
        Handling_SetToDoTaskPercentComplete_Command_With_Valid_ToDoTaskId_Should_Update_PercentComplete()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var toDoTask = ToDoTask.Create(toDoTaskId, new DateAndTime(Now.AddDays(2)),
            "Title", "Description", 80, new DateAndTime(Now));

        var command = new SetToDoTaskPercentComplete(toDoTaskId, 50);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync(toDoTask);

        var handler = new SetToDoPercentCompleteHandler(toDoTaskRepositoryMock.Object);

        // Act
        await handler.HandleAsync(command);

        // Assert 
        toDoTaskRepositoryMock.Verify(r => r.UpdateAsync(It.Is<ToDoTask>(t =>
                t.Id == new ToDoTaskId(command.ToDoTaskId) &&
                t.PercentComplete == new PercentComplete(command.PercentComplete)))
            , Times.Once);
    }

    [Fact]
    public async Task
        Handling_SetToDoTaskPercentComplete_Command_With_Nonexistent_ToDoTaskId_Should_Throw_ToDoTaskNotFoundException()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        var command = new SetToDoTaskPercentComplete(toDoTaskId, 50);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync((ToDoTask)null);

        var handler = new SetToDoPercentCompleteHandler(toDoTaskRepositoryMock.Object);

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