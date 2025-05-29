using Moq;
using Shouldly;
using ToDo.Application.Enums;
using ToDo.Application.Queries;
using ToDo.Core.Abstractions;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.DAL.Policies;
using ToDo.Infrastructure.Exceptions;
using ToDo.Infrastructure.Unit.Tests.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Handlers;

[Collection(nameof(InMemoryDbCollection))]
public class GetIncomingToDoTasksHandlerTests
{
    [Fact]
    public async Task Handling_GetIncomingToDoTasks_Query_With_Existing_Policy_Should_Return_ToDoTaskDtos()
    {
        // Arrange
        var query = new GetIncomingToDoTasks(IncomingFilter.Today);

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(_now);

        var policies = new List<IIncomingFilterPolicy>()
        {
            new TodayIncomingFilterPolicy(_dbContext, dateTimeProviderMock.Object)
        };

        var handler = new GetIncomingToDoTasksHandler(policies);

        // Act
        var result = (await handler.HandleAsync(query)).ToList();

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task
        Handling_GetIncomingToDoTasks_Query_With_Nonexistent_Policy_Should_Throw_NoIncomingFilterPolicyFoundException()
    {
        // Arrange
        var query = new GetIncomingToDoTasks(IncomingFilter.Today);

        var policies = new List<IIncomingFilterPolicy>();

        var handler = new GetIncomingToDoTasksHandler(policies);

        // Act
        var result = await Record.ExceptionAsync(async () => await handler.HandleAsync(query));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<NoIncomingFilterPolicyFoundException>();
    }

    #region ARRANGE

    private readonly ToDoDbContext _dbContext;
    private readonly DateTime _now;

    public GetIncomingToDoTasksHandlerTests(InMemoryDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _now = InMemoryDbContextFixture.Now;
    }

    #endregion
}