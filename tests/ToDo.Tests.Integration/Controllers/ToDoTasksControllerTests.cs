using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using ToDo.Application.Commands;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Tests.Integration.Controllers;

public class ToDoTasksControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    [Fact]
    public async Task PostToDoTask_WhenValidCommandIsSent_ShouldReturn201Created()
    {
        // Arrange
        var command = new CreateToDoTask(Guid.Empty, DateTime.UtcNow.AddDays(1), "Title",
            "Description", 50);

        // Act
        var response = await _client.PostAsJsonAsync("to-do-tasks", command);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
    }

    [Fact]
    public async Task PostToDoTask_WhenInvalidCommandIsSent_ShouldReturn400BadRequest()
    {
        // Arrange
        var command = new CreateToDoTask(Guid.Empty, DateTime.UtcNow.AddDays(-1), "Title",
            "Description", 50);

        // Act
        var response = await _client.PostAsJsonAsync("to-do-tasks", command);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Headers.Location.ShouldBeNull();
    }
    
    #region ARRANGE

    private readonly HttpClient _client;
    private readonly IServiceScope _scope; 
    private readonly ToDoDbContext _dbContext;
    
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