using Microsoft.EntityFrameworkCore;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.Persistence;

public class InMemoryDbContextFixture : IAsyncLifetime
{
    internal ToDoDbContext DbContext { get; set; }
    public static DateTime Now => new DateTime(2025, 4, 28, 0,0,0, DateTimeKind.Utc);

    public async Task InitializeAsync()
    {
        DbContext = await GetInMemoryDbContextAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }

    private async Task<ToDoDbContext> GetInMemoryDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ToDoDbContext(options);

        var toDoTasks = new List<ToDoTask>
        {
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddHours(6)),
                "Title 1", "Description 1", 100, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddHours(12)),
                "Title 2", "Description 2", 30, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(1)),
                "Title 3", "Description 3", 20, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(1).AddHours(6)),
                "Title 4", "Description 4", 45, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(1).AddHours(12)),
                "Title 5", "Description 5", 10, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(2)),
                "Title 6", "Description 6", 20, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(4)),
                "Title 7", "Description 7", 50, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(5)),
                "Title 8", "Description 8", 70, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(6)),
                "Title 9", "Description 9", 100, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(7)),
                "Title 10", "Description 10", 30, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(20)),
                "Title 11", "Description 11", 25, new DateAndTime(Now)),
            ToDoTask.Create(Guid.NewGuid(), new DateAndTime(Now.AddDays(30)),
                "Title 12", "Description 12", 45, new DateAndTime(Now)),
        };

        context.ToDoTasks.AddRange(toDoTasks);
        await context.SaveChangesAsync();
        return context;
    }
}