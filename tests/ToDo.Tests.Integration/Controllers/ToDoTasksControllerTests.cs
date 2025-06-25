using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using ToDo.Application.Commands;
using ToDo.Application.Common;
using ToDo.Application.DTO;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Tests.Integration.Controllers;

public class ToDoTasksControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    [Fact]
    public async Task PostToDoTask_WhenExpirationDateIsValid_ShouldReturn201Created()
    {
        // Arrange
        var command = new CreateToDoTask(Guid.Empty, DateTime.UtcNow.AddDays(1), "Title",
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
        var command = new CreateToDoTask(Guid.Empty, DateTime.UtcNow.AddDays(-1), "Title",
            "Description", 50);

        // Act
        var response = await _client.PostAsJsonAsync(ToDoTasksEndpoint, command);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Headers.Location.ShouldBeNull();
    }
    
    [Fact]
    public async Task GetAllToDoTasks_WhenPaginationParametersAreValid_ShouldReturn200Ok()
    {
        // Arrange
        var title1 = "Title1";
        var title2 = "Title2";
        var newToDoTasks = new List<ToDoTask>
        {
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(_now.AddDays(2)), title1,
                "Description 1", 50, new DateAndTime(_now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(_now.AddDays(4)), title2,
                "Description 2", 70, new DateAndTime(_now)),
        };

        await _dbContext.ToDoTasks.AddRangeAsync(newToDoTasks);
        await _dbContext.SaveChangesAsync();
        
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
        
        toDoTasksDtos.First().Title.ShouldBe(title2);
        toDoTasksDtos.Last().Title.ShouldBe(title1);
    }
    
    [Fact]
    public async Task GetAllToDoTasks_WhenPaginationParametersAreMissing_ShouldReturn400BadRequest()
    {
        // Act
        var result = await _client.GetAsync(ToDoTasksEndpoint);
        
        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Headers.Location.ShouldBeNull();
    }
    
    #region ARRANGE

    private readonly HttpClient _client;
    private readonly IServiceScope _scope; 
    private readonly ToDoDbContext _dbContext;
    private const string ToDoTasksEndpoint  = "/to-do-tasks";
    private readonly DateTime _now = DateTime.UtcNow;
    
    public ToDoTasksControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        var scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        _scope = scopeFactory.CreateScope();
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
}