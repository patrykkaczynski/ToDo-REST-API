using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shouldly;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.DAL.UnitOfWork;
using ToDo.Infrastructure.Unit.Tests.Time;

namespace ToDo.Infrastructure.Unit.Tests.DAL.UnitOfWork;

public class PostgresUnitOfWorkTests
{
    [Fact]
    public async Task ExecuteAsync_WhenActionSucceeds_ShouldCommitTransaction()
    {
        // Arrange
        var unitOfWork = new PostgresUnitOfWork(_dbContext);
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(_now.AddDays(4)),
            "Title", "Description", 50, new DateAndTime(_now));

        // Act
        await unitOfWork.ExecuteAsync(() =>
        {
            _dbContext.ToDoTasks.Add(toDoTask);
            return Task.CompletedTask;
        });

        // Assert
        var result = await _dbContext.ToDoTasks.AnyAsync(x => x.Id == toDoTask.Id);
        result.ShouldBeTrue();
    }
    
    [Fact]
    public async Task ExecuteAsync_WhenActionThrows_ShouldRollbackTransaction()
    {
        // Arrange
        var unitOfWork = new PostgresUnitOfWork(_dbContext);
        var toDoTask = ToDoTask.Create(Guid.NewGuid(), new DateAndTime(_now.AddDays(4)),
            "Title", "Description", 50, new DateAndTime(_now));

        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await unitOfWork.ExecuteAsync(() =>
            {
                _dbContext.ToDoTasks.Add(toDoTask);
                throw new Exception("Simulated failure");
            });
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<Exception>();

        var result = await _dbContext.ToDoTasks.AnyAsync(x => x.Id == toDoTask.Id);
        result.ShouldBeFalse();
    }
    
    #region ARRANGE

    private readonly ToDoDbContext _dbContext;
    private readonly DateTime _now;

    public PostgresUnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ToDoDbContext(options);
        _now = new TestDateTimeProvider().Current();
    }

    #endregion
}