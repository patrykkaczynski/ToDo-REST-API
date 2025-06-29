using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using ToDo.Application.Commands;
using ToDo.Application.Common;
using ToDo.Application.DTO;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Integration.Tests.Time;

namespace ToDo.Integration.Tests.Controllers;

public class ToDoTasksControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    [Fact]
    public async Task GetToDoTasks_WhenPaginationParametersAreValid_ShouldReturn200Ok()
    {
        // Arrange
        var toDoTask1 = await AddToDoTaskAsync(Guid.NewGuid(), _now.AddDays(2), "Title 1",
            "Description 1", 50);
        var toDoTask2 = await AddToDoTaskAsync(Guid.NewGuid(), _now.AddDays(4), "Title 2",
            "Description 2", 70);

        // Act
        var response = await _client
            .GetAsync($"{ToDoTasksEndpoint}?pageNumber=1&pageSize=10&sortBy=ExpirationDate&sortDirection=Descending");
        var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<ToDoTaskDto>>();
        var toDoTasksDtos = pagedResult.Items.ToList();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location.ShouldBeNull();

        pagedResult.ShouldNotBeNull();
        pagedResult.TotalPages.ShouldBe(1);
        pagedResult.TotalItemsCount.ShouldBe(2);
        pagedResult.ItemsFrom.ShouldBe(1);
        pagedResult.ItemsTo.ShouldBe(10);
        pagedResult.Items.ToList().Count.ShouldBe(2);

        toDoTasksDtos.First().Title.ShouldBe(toDoTask2.Title);
        toDoTasksDtos.Last().Title.ShouldBe(toDoTask1.Title);
    }

    [Fact]
    public async Task GetToDoTasks_WhenPaginationParametersAreMissing_ShouldReturn400BadRequest()
    {
        // Act
        var response = await _client.GetAsync(ToDoTasksEndpoint);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task GetToDoTask_WhenToDoTaskIdExists_ShouldReturn200Ok()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var title = "Title";
        var toDoTask = await AddToDoTaskAsync(toDoTaskId, _now.AddDays(2), title, "Description", 50);

        // Act
        var response = await _client.GetAsync($"{ToDoTasksEndpoint}/{toDoTaskId}");
        var toDoTaskDto = await response.Content.ReadFromJsonAsync<ToDoTaskDto>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location.ShouldBeNull();
        toDoTaskDto.Title.ShouldBe(toDoTask.Title);
    }

    [Fact]
    public async Task GetToDoTask_WhenToDoTaskIdDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"{ToDoTasksEndpoint}/{toDoTaskId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task GetIncomingToDoTasks_WhenIncomingFilterExists_ShouldReturn200Ok()
    {
        // Arrange
        var title = "Today Task";
        await AddToDoTaskAsync(Guid.NewGuid(), _now.AddHours(5), title, "Description 1", 30);
        await AddToDoTaskAsync(Guid.NewGuid(), _now.AddDays(1), "Title 2", "Description 2", 50);
        await AddToDoTaskAsync(Guid.NewGuid(), _now.AddDays(4), "Title 3", "Description 3", 70);

        // Act
        var response = await _client.GetAsync($"{ToDoTasksEndpoint}/incoming?incomingFilter=Today");
        var toDoTasksDtos = (await response.Content.ReadFromJsonAsync<IEnumerable<ToDoTaskDto>>())
            .ToList();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location.ShouldBeNull();

        toDoTasksDtos.Count.ShouldBe(1);
        toDoTasksDtos.First().Title.ShouldBe(title);
    }

    [Theory]
    [InlineData("CurrentYear", HttpStatusCode.BadRequest)]
    [InlineData("None", HttpStatusCode.NotFound)]
    public async Task GetIncomingToDoTasks_WhenIncomingFilterIsInvalid_ShouldReturnExpectedStatus(string filter,
        HttpStatusCode expectedStatus)
    {
        // Act
        var response = await _client.GetAsync($"{ToDoTasksEndpoint}/incoming?incomingFilter={filter}");

        // Assert
        response.StatusCode.ShouldBe(expectedStatus);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task PostToDoTask_WhenExpirationDateIsValid_ShouldReturn201Created()
    {
        // Arrange
        var command = new CreateToDoTask(Guid.Empty, _now.AddDays(1), "Title",
            "Description", 50);

        // Act
        var response = await _client.PostAsJsonAsync(ToDoTasksEndpoint, command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldContain(ToDoTasksEndpoint);
    }

    [Fact]
    public async Task PostToDoTask_WhenExpirationDateIsInThePast_ShouldReturn400BadRequest()
    {
        // Arrange
        var command = new CreateToDoTask(Guid.Empty, _now.AddDays(-1), "Title",
            "Description", 50);

        // Act
        var response = await _client.PostAsJsonAsync(ToDoTasksEndpoint, command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateToDoTask_WhenToDoTaskIdExists_ShouldReturn200Ok()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var toDoTask = await AddToDoTaskAsync(toDoTaskId, _now.AddDays(2), "Title",
            "Description", 50);
        var command = new UpdateToDoTask(toDoTaskId, toDoTask.ExpirationDate.Value.DateTime, "Updated Title",
            toDoTask.Description, toDoTask.PercentComplete);

        // Act
        var response = await _client.PutAsJsonAsync($"{ToDoTasksEndpoint}/{toDoTaskId}", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateToDoTask_WhenExpirationDateIsInThePast_ShouldReturn400BadRequest()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var updatedExpirationDate = _now.AddDays(-2);
        var toDoTask = await AddToDoTaskAsync(toDoTaskId, _now.AddDays(2), "Title",
            "Description", 50);

        var command = new UpdateToDoTask(toDoTaskId, updatedExpirationDate, toDoTask.Title,
            toDoTask.Description, toDoTask.PercentComplete);

        // Act
        var response = await _client.PutAsJsonAsync($"{ToDoTasksEndpoint}/{toDoTaskId}", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateToDoTask_WhenToDoTaskIdDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var command = new UpdateToDoTask(toDoTaskId, _now.AddDays(2), "Title",
            "Description", 50);

        // Act
        var response = await _client.PutAsJsonAsync($"{ToDoTasksEndpoint}/{toDoTaskId}", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task SetToDoTaskPercentComplete_WhenToDoTaskIdExists_ShouldReturn200Ok()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var updatedPercentComplete = 70;
        await AddToDoTaskAsync(toDoTaskId, _now.AddDays(2), "Title",
            "Description", 50);
        var command = new SetToDoTaskPercentComplete(toDoTaskId, updatedPercentComplete);

        // Act
        var response = await _client.PatchAsJsonAsync($"{ToDoTasksEndpoint}/{toDoTaskId}/percent-complete", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location.ShouldBeNull();

        var result = await _dbContext.ToDoTasks
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == new ToDoTaskId(toDoTaskId));
        result.PercentComplete.Value.ShouldBe(updatedPercentComplete);
    }

    [Fact]
    public async Task SetToDoTaskPercentComplete_WhenPercentCompleteIsInvalid_ShouldReturn400BadRequest()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var updatedPercentComplete = -20;
        await AddToDoTaskAsync(toDoTaskId, _now.AddDays(2), "Title",
            "Description", 50);

        var command = new SetToDoTaskPercentComplete(toDoTaskId, updatedPercentComplete);

        // Act
        var response = await _client.PatchAsJsonAsync($"{ToDoTasksEndpoint}/{toDoTaskId}/percent-complete", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task SetToDoTaskPercentComplete_WhenToDoTaskIdDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        var command = new SetToDoTaskPercentComplete(toDoTaskId, 20);

        // Act
        var response = await _client.PatchAsJsonAsync($"{ToDoTasksEndpoint}/{toDoTaskId}/percent-complete", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteToDoTask_WhenToDoTaskIdExists_ShouldReturn204NoContent()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        await AddToDoTaskAsync(toDoTaskId, _now.AddDays(2), "Title",
            "Description", 50);

        // Act
        var response = await _client.DeleteAsync($"{ToDoTasksEndpoint}/{toDoTaskId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteToDoTask_WhenToDoTaskIdDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"{ToDoTasksEndpoint}/{toDoTaskId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task MarkToDoTaskAsDone_WhenToDoTaskIdExists_ShouldReturn200Ok()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();
        await AddToDoTaskAsync(toDoTaskId,_now.AddDays(2), "Title",
            "Description", 50);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{ToDoTasksEndpoint}/{toDoTaskId}/done");
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location.ShouldBeNull();
    }

    [Fact]
    public async Task MarkToDoTaskAsDone_WhenToDoTaskIdDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var toDoTaskId = Guid.NewGuid();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{ToDoTasksEndpoint}/{toDoTaskId}/done");
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Headers.Location.ShouldBeNull();
    }

    #region ARRANGE

    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ToDoDbContext _dbContext;
    private const string ToDoTasksEndpoint = "/to-do-tasks";
    private readonly DateTime _now = new TestDateTimeProvider().Current();

    public ToDoTasksControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        _dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _dbContext?.Database.EnsureDeleted();
        _dbContext?.Dispose();
        _scope?.Dispose();
    }

    #endregion
    
    #region HELPERS
    
    private async Task<ToDoTask> AddToDoTaskAsync(Guid toDoTaskId, DateTime expirationDate, string title,
        string description, int percentComplete)
    {
        var task = ToDoTask.Create(toDoTaskId, new DateAndTime(expirationDate), title, description,
            percentComplete, new DateAndTime(_now));

        await _dbContext.ToDoTasks.AddAsync(task);
        await _dbContext.SaveChangesAsync();
        return task;
    }
    
    #endregion
}