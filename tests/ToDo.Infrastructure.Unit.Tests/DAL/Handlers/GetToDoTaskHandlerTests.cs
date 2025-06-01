using Shouldly;
using ToDo.Application.Exceptions;
using ToDo.Application.Queries;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.Unit.Tests.Base;
using ToDo.Infrastructure.Unit.Tests.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Handlers;

[Collection(nameof(InMemoryDbCollection))]
public class GetToDoTaskHandlerTests : TestBase
{
    public GetToDoTaskHandlerTests(InMemoryDbContextFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData("240d2448-4c52-4fce-a26d-2a78a97fd56e", "Title 1")]
    [InlineData("940521da-1c33-4ec7-a869-3c29ef3cf05f", "Title 6")]
    [InlineData("6e96bb1e-3089-4817-9019-7da90af741ca", "Title 12")]
    public async Task Handling_GetToDoTask_Query_With_Existing_ToDoTaskId_Should_Return_ToDoTaskDto(string id,
        string expectedTitle)
    {
        // Arrange
        var toDoTaskId = Guid.Parse(id);

        var query = new GetToDoTask(toDoTaskId);

        var handler = new GetToDoTaskHandler(_dbContext);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe(expectedTitle);
    }

    [Fact]
    public async Task Handling_GetToDoTask_Query_With_Nonexistent_ToDoTaskId_Should_Throw_ToDoTaskNotFoundException()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        var query = new GetToDoTask(toDoTaskId);

        var handler = new GetToDoTaskHandler(_dbContext);

        // Act
        var result = await Record.ExceptionAsync(async () => await handler.HandleAsync(query));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<ToDoTaskNotFoundException>();
    }
}