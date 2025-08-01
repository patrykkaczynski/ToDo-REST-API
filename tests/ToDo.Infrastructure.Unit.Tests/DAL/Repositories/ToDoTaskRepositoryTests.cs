using Microsoft.EntityFrameworkCore;
using Shouldly;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.DAL.Repositories;
using ToDo.Infrastructure.Unit.Tests.Time;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Repositories;

public class ToDoTaskRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenToDoTaskExists_ShouldReturnToDoTask()
    {
        // Arrange
        var repository = new ToDoTaskRepository(_dbContext);
        var expectedToDoTask = ToDoTask.Create(Guid.Parse("6b1a6e07-9957-4149-a5f1-2f8cfaafb1b6"),
            new DateAndTime(_now.AddDays(4)),
            "Title 7", "Description 7", 50, new DateAndTime(_now));
        await _dbContext.ToDoTasks.AddAsync(expectedToDoTask);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(expectedToDoTask.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(expectedToDoTask.Id);
        result.ExpirationDate.ShouldBe(expectedToDoTask.ExpirationDate);
        result.Title.ShouldBe(expectedToDoTask.Title);
        result.Description.ShouldBe(expectedToDoTask.Description);
        result.PercentComplete.ShouldBe(expectedToDoTask.PercentComplete);
    }

    [Fact]
    public async Task GetByIdAsync_WhenToDoTaskDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var repository = new ToDoTaskRepository(_dbContext);
        var toDoTaskId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(toDoTaskId);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task CreateAsync_WhenToDoTaskIsValid_ShouldAddItToDatabase()
    {
        // Arrange
        var repository = new ToDoTaskRepository(_dbContext);
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(_now.AddDays(2)),
            "Title", "Description", 20, new DateAndTime(_now));

        // Act
        await repository.CreateAsync(toDoTask);
        await _dbContext.SaveChangesAsync();

        var savedToDoTask = await _dbContext.ToDoTasks.FindAsync(toDoTask.Id);

        // Assert
        savedToDoTask.ShouldNotBeNull();
        savedToDoTask.Id.ShouldBe(toDoTask.Id);
        savedToDoTask.ExpirationDate.ShouldBe(toDoTask.ExpirationDate);
        savedToDoTask.Title.ShouldBe(toDoTask.Title);
        savedToDoTask.Description.ShouldBe(toDoTask.Description);
        savedToDoTask.PercentComplete.ShouldBe(toDoTask.PercentComplete);
    }

    [Fact]
    public async Task UpdateAsync_WhenToDoTaskExists_ShouldModifyItInDatabase()
    {
        // Arrange
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(_now.AddDays(2)),
            "Title", "Description", 10, new DateAndTime(_now));

        await _dbContext.ToDoTasks.AddAsync(toDoTask);
        await _dbContext.SaveChangesAsync();

        var repository = new ToDoTaskRepository(_dbContext);

        // Act
        toDoTask.Update(new DateAndTime(_now.AddDays(4)), "Updated Title",
            "Updated Description", 30, new DateAndTime(_now));
        await repository.UpdateAsync(toDoTask);
        await _dbContext.SaveChangesAsync();

        var updatedToDoTask = await _dbContext.ToDoTasks.FindAsync(toDoTask.Id);

        // Assert
        updatedToDoTask.ShouldNotBeNull();
        updatedToDoTask.Id.ShouldBe(toDoTask.Id);
        updatedToDoTask.ExpirationDate.ShouldBe(toDoTask.ExpirationDate);
        updatedToDoTask.Title.ShouldBe(toDoTask.Title);
        updatedToDoTask.Description.ShouldBe(toDoTask.Description);
        updatedToDoTask.PercentComplete.ShouldBe(toDoTask.PercentComplete);
    }

    [Fact]
    public async Task DeleteAsync_WhenToDoTaskExists_ShouldRemoveItFromDatabase()
    {
        // Arrange
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(_now.AddDays(2)),
            "Title", "Description", 0, new DateAndTime(_now));
    
        await _dbContext.ToDoTasks.AddAsync(toDoTask);
        await _dbContext.SaveChangesAsync();
        
        var repository = new ToDoTaskRepository(_dbContext);
    
        // Act
        await repository.DeleteAsync(toDoTask);
        await _dbContext.SaveChangesAsync();
    
        var deletedToDoTask = await _dbContext.ToDoTasks.FindAsync(toDoTask.Id);
    
        // Assert
        deletedToDoTask.ShouldBeNull();
    }

    #region ARRANGE

    private readonly ToDoDbContext _dbContext;
    private readonly DateTime _now;

    public ToDoTaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ToDoDbContext(options);
        _now = new TestDateTimeProvider().Current();
    }

    #endregion
}