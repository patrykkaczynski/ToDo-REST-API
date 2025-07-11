using Moq;
using Shouldly;
using ToDo.Application.Commands;
using ToDo.Application.Commands.Handlers;
using ToDo.Application.Exceptions;
using ToDo.Application.Unit.Tests.Time;
using ToDo.Core.Abstractions;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Unit.Tests.Commands.Handlers;

public class UpdateToDoTaskHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenUpdateToDoTaskCommandWithValidToDoTaskIdIsHandled_ShouldUpdateToDoTask()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var toDoTask = ToDoTask.Create(toDoTaskId, new DateAndTime(_now.AddDays(2)),
            "Title", "Description", 20, new DateAndTime(_now));

        var command = new UpdateToDoTask(toDoTaskId, _now.AddDays(3),
            "Updated Title", "Updated Description", 80);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync(toDoTask);

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(_now);

        var handler = new UpdateToDoTaskHandler(toDoTaskRepositoryMock.Object, dateTimeProviderMock.Object);

        // Act
        await handler.HandleAsync(command);

        // Assert 
        toDoTaskRepositoryMock.Verify(r => r.UpdateAsync(It.Is<ToDoTask>(t =>
                t.Id == new ToDoTaskId(command.ToDoTaskId) &&
                t.ExpirationDate == new DateAndTime(command.ExpirationDate) &&
                t.Title == new Title(command.Title) &&
                t.Description == new Description(command.Description) &&
                t.PercentComplete == new PercentComplete(command.PercentComplete)))
            , Times.Once);
    }

    [Fact]
    public async Task
        HandleAsync_WhenUpdateToDoTaskCommandWithNonexistentToDoTaskIdIsHandled_ShouldThrowToDoTaskNotFoundException()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        var command = new UpdateToDoTask(toDoTaskId, _now.AddDays(3),
            "Updated Title", "Updated Description", 80);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();
        toDoTaskRepositoryMock.Setup(r => r.GetByIdAsync(toDoTaskId))
            .ReturnsAsync((ToDoTask)null);

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(_now);

        var handler = new UpdateToDoTaskHandler(toDoTaskRepositoryMock.Object, dateTimeProviderMock.Object);

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