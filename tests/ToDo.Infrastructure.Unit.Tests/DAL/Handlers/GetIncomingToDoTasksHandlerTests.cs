using Moq;
using Shouldly;
using ToDo.Application.Common;
using ToDo.Application.Queries;
using ToDo.Core.Abstractions;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Policies;
using ToDo.Infrastructure.Exceptions;
using ToDo.Infrastructure.Unit.Tests.DAL.Base;
using ToDo.Infrastructure.Unit.Tests.DAL.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Handlers;

[Collection(nameof(InMemoryDbCollection))]
public class GetIncomingToDoTasksHandlerTests(InMemoryDbContextFixture fixture) : TestBase(fixture)
{
    [Fact]
    public async Task HandleAsync_WhenExistingPolicyIsProvided_ShouldReturnToDoTaskDtos()
    {
        // Arrange
        var query = new GetIncomingToDoTasks(IncomingFilter.Today);

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(Now);

        var policies = new List<IIncomingFilterPolicy>()
        {
            new TodayIncomingFilterPolicy(DbContext, dateTimeProviderMock.Object)
        };

        var handler = new GetIncomingToDoTasksHandler(policies);

        // Act
        var result = (await handler.HandleAsync(query)).ToList();

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task HandleAsync_WhenNoPolicyIsProvided_ShouldThrowNoIncomingFilterPolicyFoundException()
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
}