using Moq;
using Shouldly;
using ToDo.Application.Enums;
using ToDo.Core.Abstractions;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.DAL.Policies;
using ToDo.Infrastructure.Unit.Tests.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Policies;

[Collection(nameof(InMemoryDbCollection))]
public class TomorrowIncomingFilterPolicyTests
{
    [Theory]
    [InlineData(IncomingFilter.CurrentWeek, false)]
    [InlineData(IncomingFilter.Today, false)]
    [InlineData(IncomingFilter.Tomorrow, true)]
    public void CanBeApplied_With_Various_Filters_Should_Return_Expected_Result(IncomingFilter incomingFilter,
        bool expectedResult)
    {
        // Arrange
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        
        var policy = new TomorrowIncomingFilterPolicy(_dbContext, dateTimeProviderMock.Object);

        // Act 
        var result = policy.CanBeApplied(incomingFilter);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task GetIncomingToDoTasks_WithTomorrowPolicy_Should_Return_Only_ToDoTaskDtos_ForTomorrow()
    {
        // Arrange
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.Current())
            .Returns(_now);
        
        var policy = new TomorrowIncomingFilterPolicy(_dbContext, dateTimeProviderMock.Object);

        // Act 
        var result = (await policy.GetIncomingToDoTasksAsync()).ToList();

        // Assert
        var expectedTitles = new[] { "Title 3", "Title 4", "Title 5"};
        
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(3);
        result.Select(t => t.Title).ShouldBe(expectedTitles, ignoreOrder: true);
    }

    #region ARRANGE

    private readonly ToDoDbContext _dbContext;
    private readonly DateTime _now;
    public TomorrowIncomingFilterPolicyTests(InMemoryDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _now = InMemoryDbContextFixture.Now;
    }
    
    #endregion
}