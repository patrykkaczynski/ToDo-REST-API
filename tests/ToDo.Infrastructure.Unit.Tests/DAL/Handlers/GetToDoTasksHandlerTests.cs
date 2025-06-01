using Shouldly;
using ToDo.Application.Common;
using ToDo.Application.DTO;
using ToDo.Application.Queries;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.Exceptions;
using ToDo.Infrastructure.Unit.Tests.DAL.Base;
using ToDo.Infrastructure.Unit.Tests.DAL.Persistence;
using SortDirection = ToDo.Application.Common.SortDirection;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Handlers;

[Collection(nameof(InMemoryDbCollection))]
public class GetToDoTasksHandlerTests(InMemoryDbContextFixture fixture) : TestBase(fixture)
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(100)]
    public async Task Handling_GetToDoTasks_Query_With_Invalid_PageSize_Should_Throw_InvalidPageSizeException
        (int pageSize)
    {
        // Arrange
        var query = new GetToDoTasks(null, 1, pageSize, null, SortDirection.Ascending);

        var handler = new GetToDoTasksHandler(DbContext);

        // Act
        var result = await Record.ExceptionAsync(async () => await handler.HandleAsync(query));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<InvalidPageSizeException>();
    }

    [Fact]
    public async Task Handling_GetToDoTasks_Query_With_NonMatching_SearchPhrase_Should_Return_Empty_PagedResult()
    {
        // Arrange
        var query = new GetToDoTasks("Wrong search phrase", 1, 5, null, SortDirection.Ascending);

        var handler = new GetToDoTasksHandler(DbContext);

        var expectedResult = new PagedResult<ToDoTaskDto>
        {
            Items = [],
            TotalItemsCount = 0,
            TotalPages = 0,
            ItemsFrom = 0,
            ItemsTo = 0
        };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBe(expectedResult.Items);
        result.TotalItemsCount.ShouldBe(expectedResult.TotalItemsCount);
        result.TotalPages.ShouldBe(expectedResult.TotalPages);
        result.ItemsFrom.ShouldBe(expectedResult.ItemsFrom);
        result.ItemsTo.ShouldBe(expectedResult.ItemsTo);
    }

    [Theory]
    [InlineData(5, 0)]
    [InlineData(5, 4)]
    [InlineData(10, 0)]
    [InlineData(10, 3)]
    [InlineData(25, 0)]
    [InlineData(25, 2)]
    [InlineData(50, 0)]
    [InlineData(50, 2)]
    public async Task Handling_GetToDoTasks_Query_With_Invalid_PageNumber_Should_Throw_InvalidPageNumberException
        (int pageSize, int pageNumber)
    {
        // Arrange
        var query = new GetToDoTasks("Title", pageNumber, pageSize, null, SortDirection.Ascending);

        var handler = new GetToDoTasksHandler(DbContext);

        // Act
        var result = await Record.ExceptionAsync(async () => await handler.HandleAsync(query));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<InvalidPageNumberException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("columnName")]
    [InlineData("text")]
    [InlineData("eeeexpirationDate")]
    [InlineData("titlee")]
    [InlineData("Desc")]
    [InlineData("ppercentCComplete")]
    public async Task
        Handling_GetToDoTasks_Query_With_Invalid_SortByColumnName_Should_Throw_InvalidSortByColumnNameException
        (string sortByColumnName)
    {
        // Arrange
        var query = new GetToDoTasks("Title", 1, 5, sortByColumnName, SortDirection.Ascending);

        var handler = new GetToDoTasksHandler(DbContext);

        // Act
        var result = await Record.ExceptionAsync(async () => await handler.HandleAsync(query));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<InvalidSortByColumnNameException>();
    }

    [Theory]
    [InlineData("1", 1, 5, "ExpirationDate",
        SortDirection.Ascending, 7, "Title 1", "Title 13")]
    [InlineData("1", 2, 5, "ExpirationDate",
        SortDirection.Ascending, 7, "Title 14", "Title 15")]
    [InlineData("1", 1, 5, "ExpirationDate",
        SortDirection.Descending, 7, "Title 15", "Title 11")]
    [InlineData("1", 2, 5, "ExpirationDate",
        SortDirection.Descending, 7, "Title 10", "Title 1")]
    [InlineData("1", 1, 10, "ExpirationDate",
        SortDirection.Ascending, 7, "Title 1", "Title 15")]
    [InlineData("1", 1, 10, "ExpirationDate",
        SortDirection.Descending, 7, "Title 15", "Title 1")]
    [InlineData("1", 1, 25, "ExpirationDate",
        SortDirection.Ascending, 7, "Title 1", "Title 15")]
    [InlineData("1", 1, 25, "ExpirationDate",
        SortDirection.Descending, 7, "Title 15", "Title 1")]
    [InlineData("1", 1, 50, "ExpirationDate",
        SortDirection.Ascending, 7, "Title 1", "Title 15")]
    [InlineData("1", 1, 50, "ExpirationDate",
        SortDirection.Descending, 7, "Title 15", "Title 1")]
    [InlineData(null, 1, 5, "ExpirationDate",
        SortDirection.Ascending, 15, "Title 1", "Title 5")]
    [InlineData(null, 2, 5, "ExpirationDate",
        SortDirection.Ascending, 15, "Title 6", "Title 10")]
    [InlineData(null, 3, 5, "ExpirationDate",
        SortDirection.Ascending, 15, "Title 11", "Title 15")]
    [InlineData(null, 1, 5, "ExpirationDate",
        SortDirection.Descending, 15, "Title 15", "Title 11")]
    [InlineData(null, 2, 5, "ExpirationDate",
        SortDirection.Descending, 15, "Title 10", "Title 6")]
    [InlineData(null, 3, 5, "ExpirationDate",
        SortDirection.Descending, 15, "Title 5", "Title 1")]
    public async Task Handling_GetToDoTasks_Query_With_Valid_Data_Should_Returns_PaginatedResult
    (string searchPhrase, int pageNumber, int pageSize, string sortByColumnName, SortDirection sortDirection,
        int totalCount, string firstTitle, string lastTitle)
    {
        // Arrange
        var query = new GetToDoTasks(searchPhrase, pageNumber, pageSize, sortByColumnName, sortDirection);

        var handler = new GetToDoTasksHandler(DbContext);

        // Act
        var result = await handler.HandleAsync(query);
        var itemsResult = result.Items.ToList();

        // Assert
        result.ShouldNotBeNull();
        result.TotalItemsCount.ShouldBe(totalCount);
        itemsResult.First().Title.ShouldBe(firstTitle);
        itemsResult.Last().Title.ShouldBe(lastTitle);
    }
}