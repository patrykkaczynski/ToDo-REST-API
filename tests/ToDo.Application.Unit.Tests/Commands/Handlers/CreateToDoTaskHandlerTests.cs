using Moq;
using ToDo.Application.Commands;
using ToDo.Application.Commands.Handlers;
using ToDo.Application.Unit.Tests.Time;
using ToDo.Core.Abstractions;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Unit.Tests.Commands.Handlers;

public class CreateToDoTaskHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenCreateToDoTaskCommandIsValid_ShouldCreateToDoTask()
    {
        // Arrange
        var command = new CreateToDoTask(Guid.NewGuid(), _now.AddDays(1), "Title",
            "Description", 50);

        var toDoTaskRepositoryMock = new Mock<IToDoTaskRepository>();

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(_now);
        
        var handler = new CreateToDoTaskHandler(toDoTaskRepositoryMock.Object, dateTimeProviderMock.Object);

        // Act
        await handler.HandleAsync(command);

        // Assert
        toDoTaskRepositoryMock.Verify(r => r.CreateAsync(It.Is<ToDoTask>(t =>
            t.Id == new ToDoTaskId(command.ToDoTaskId) &&
            t.ExpirationDate == new DateAndTime(command.ExpirationDate) &&
            t.Title == new Title(command.Title) &&
            t.Description == new Description(command.Description) &&
            t.PercentComplete == new PercentComplete(command.PercentComplete)
        )), Times.Once);
    }

    #region ARRANGE

    private readonly DateTime _now = new TestDateTimeProvider().Current();

    #endregion
}