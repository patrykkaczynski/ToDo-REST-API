using Moq;
using Shouldly;
using ToDo.Application.Commands;
using ToDo.Application.Commands.Handlers;
using ToDo.Application.Exceptions;
using ToDo.Application.Unit.Tests.Time;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Unit.Tests.Commands.Handlers;

public class DeleteToDoTaskHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenDeleteToDoTaskCommandWithValidToDoTaskIdIsHandled_ShouldDeleteToDoTask()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var toDoTask = ToDoTask.Create(toDoTaskId, new DateAndTime(_now.AddDays(2)),
            "Title", "Description", 80, new DateAndTime(_now));

        var command = new DeleteToDoTask(toDoTaskId);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync(toDoTask);

        var handler = new DeleteToDoTaskHandler(toDoTaskRepositoryMock.Object);

        // Act
        await handler.HandleAsync(command);

        // Assert
        toDoTaskRepositoryMock.Verify(r => r.DeleteAsync(It.Is<ToDoTask>(t =>
                t.Id == new ToDoTaskId(command.ToDoTaskId))),
            Times.Once);
    }

    [Fact]
    public async Task
        HandleAsync_WhenDeleteToDoTaskCommandWithNonexistentToDoTaskIdIsHandled_ShouldThrowToDoTaskNotFoundException()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        var command = new DeleteToDoTask(toDoTaskId);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync((ToDoTask)null);

        var handler = new DeleteToDoTaskHandler(toDoTaskRepositoryMock.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(command));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<ToDoTaskNotFoundException>();
    }

    #region ARRANGE

    private readonly DateTime _now = new TestDateTimeProvider().Current();

    #endregion
}