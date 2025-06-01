using Moq;
using Shouldly;
using ToDo.Application.Common;
using ToDo.Core.Abstractions;
using ToDo.Infrastructure.DAL.Policies;
using ToDo.Infrastructure.Unit.Tests.Base;
using ToDo.Infrastructure.Unit.Tests.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Policies;

[Collection(nameof(InMemoryDbCollection))]
public class CurrentWeekIncomingFilterPolicyTests : TestBase
{
    public CurrentWeekIncomingFilterPolicyTests(InMemoryDbContextFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(IncomingFilter.CurrentWeek, true)]
    [InlineData(IncomingFilter.Today, false)]
    [InlineData(IncomingFilter.Tomorrow, false)]
    public void CanBeApplied_With_Various_Filters_Should_Return_Expected_Result(IncomingFilter incomingFilter,
        bool expectedResult)
    {
        // Arrange
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var policy = new CurrentWeekIncomingFilterPolicy(_dbContext, dateTimeProviderMock.Object);

        // Act 
        var result = policy.CanBeApplied(incomingFilter);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task GetIncomingToDoTasks_WithCurrentWeekPolicy_Should_Return_Only_ToDoTaskDtos_Within_ThisWeek()
    {
        // Arrange
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(_now);

        var policy = new CurrentWeekIncomingFilterPolicy(_dbContext, dateTimeProviderMock.Object);

        // Act 
        var result = (await policy.GetIncomingToDoTasksAsync()).ToList();

        // Assert
        var expectedTitles = new[]
        {
            "Title 1", "Title 2", "Title 3", "Title 4", "Title 5", "Title 6", "Title 7", "Title 8", "Title 9"
        };

        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(9);
        result.Select(t => t.Title).ShouldBe(expectedTitles, ignoreOrder: true);
    }
}