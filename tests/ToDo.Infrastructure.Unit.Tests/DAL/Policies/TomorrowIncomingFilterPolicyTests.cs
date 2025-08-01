using Moq;
using Shouldly;
using ToDo.Application.Common;
using ToDo.Core.Abstractions;
using ToDo.Infrastructure.DAL.Policies;
using ToDo.Infrastructure.Unit.Tests.DAL.Base;
using ToDo.Infrastructure.Unit.Tests.DAL.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Policies;

[Collection(nameof(InMemoryDbCollection))]
public class TomorrowIncomingFilterPolicyTests(InMemoryDbContextFixture fixture) : TestBase(fixture)
{
    [Theory]
    [InlineData(IncomingFilter.CurrentWeek, false)]
    [InlineData(IncomingFilter.Today, false)]
    [InlineData(IncomingFilter.Tomorrow, true)]
    public void CanBeApplied_WhenUsingVariousFilters_ShouldReturnExpectedResults(IncomingFilter incomingFilter,
        bool expectedResult)
    {
        // Arrange
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var policy = new TomorrowIncomingFilterPolicy(DbContext, dateTimeProviderMock.Object);

        // Act 
        var result = policy.CanBeApplied(incomingFilter);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task GetIncomingToDoTasksAsync_WhenTomorrowPolicyIsApplied_ShouldReturnOnlyTomorrowToDoTasks()
    {
        // Arrange
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(Now);

        var policy = new TomorrowIncomingFilterPolicy(DbContext, dateTimeProviderMock.Object);

        // Act 
        var result = (await policy.GetIncomingToDoTasksAsync()).ToList();

        // Assert
        var expectedTitles = new[] { "Title 3", "Title 4", "Title 5" };

        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(3);
        result.Select(t => t.Title).ShouldBe(expectedTitles, ignoreOrder: true);
    }
}