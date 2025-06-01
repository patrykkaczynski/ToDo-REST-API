using Microsoft.EntityFrameworkCore;
using Shouldly;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.DAL.Repositories;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Repositories;

public class ToDoTaskRepositoryTests
{
    [Fact]
    public async Task GetById_With_Existing_ToDoTaskId_Should_Return_ToDoTask()
    {
        // Arrange
        var repository = new ToDoTaskRepository(_dbContext);
        var expectedToDoTask = ToDoTask.Create(Guid.Parse("6b1a6e07-9957-4149-a5f1-2f8cfaafb1b6"),
            new DateAndTime(Now.AddDays(4)),
            "Title 7", "Description 7", 50, new DateAndTime(Now));
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
    public async Task GetById_With_Nonexistent_ToDoTaskId_Should_Return_Null()
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
    public async Task Create_Should_Add_ToDoTask_To_Database()
    {
        // Arrange
        var repository = new ToDoTaskRepository(_dbContext);
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(2)),
            "Title", "Description", 20, new DateAndTime(Now));

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
    public async Task Update_Should_Modify_ToDoTask_In_Database()
    {
        // Arrange
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(2)),
            "Title", "Description", 10, new DateAndTime(Now));

        await _dbContext.ToDoTasks.AddAsync(toDoTask);
        await _dbContext.SaveChangesAsync();

        var repository = new ToDoTaskRepository(_dbContext);

        // Act
        toDoTask.Update(new DateAndTime(Now.AddDays(4)), "Updated Title",
            "Updated Description", 30, new DateAndTime(Now));
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
    public async Task Delete_Should_Remove_ToDoTask_From_Database()
    {
        // Arrange
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(2)),
            "Title", "Description", 0, new DateAndTime(Now));
    
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
    private static DateTime Now => new DateTime(2025, 4, 28, 0, 0, 0, DateTimeKind.Utc);

    public ToDoTaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ToDoDbContext(options);
    }

    #endregion
}