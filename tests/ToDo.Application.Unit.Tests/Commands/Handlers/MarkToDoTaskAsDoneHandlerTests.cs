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

public class MarkToDoTaskAsDoneHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenMarkToDoTaskAsDoneCommandWithValidToDoTaskIdIsHandled_ShouldMarkAsDone()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var percentComplete = new PercentComplete(100);

        var toDoTask = ToDoTask.Create(toDoTaskId, new DateAndTime(_now.AddDays(2)),
            "Title", "Description", 50, new DateAndTime(_now));

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
        HandleAsync_WhenMarkToDoTaskAsDoneCommandWithNonexistentToDoTaskIdIsHandled_ShouldThrowToDoTaskNotFoundException()
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

    private readonly DateTime _now = new TestDateTimeProvider().Current();

    #endregion
}