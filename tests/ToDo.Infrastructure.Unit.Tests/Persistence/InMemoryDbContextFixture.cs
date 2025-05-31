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
            ToDoTask.Create(Guid.Parse("240d2448-4c52-4fce-a26d-2a78a97fd56e"), new DateAndTime(Now.AddHours(6)),
                "Title 1", "Description 1", 100, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("cf21db71-decd-44b9-af10-50817cb4867f"), new DateAndTime(Now.AddHours(12)),
                "Title 2", "Description 2", 30, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("9ce97ca3-4b68-4c3e-8643-448af6be8a86"), new DateAndTime(Now.AddDays(1)),
                "Title 3", "Description 3", 20, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("767b0e75-4bed-40be-a19f-7a0a338757b7"), new DateAndTime(Now.AddDays(1).AddHours(6)),
                "Title 4", "Description 4", 45, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("ff43d464-1aa7-4241-9cb4-353f1c159448"), new DateAndTime(Now.AddDays(1).AddHours(12)),
                "Title 5", "Description 5", 10, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("940521da-1c33-4ec7-a869-3c29ef3cf05f"), new DateAndTime(Now.AddDays(2)),
                "Title 6", "Description 6", 20, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("6b1a6e07-9957-4149-a5f1-2f8cfaafb1b6"), new DateAndTime(Now.AddDays(4)),
                "Title 7", "Description 7", 50, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("74ba8e1a-74eb-4819-b1dd-03d27c500cee"), new DateAndTime(Now.AddDays(5)),
                "Title 8", "Description 8", 70, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("358d76a4-12b0-4dcb-b0a5-52092e3b50ba"), new DateAndTime(Now.AddDays(6)),
                "Title 9", "Description 9", 100, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("a310f0fb-d345-45fd-ac8b-bd506449b929"), new DateAndTime(Now.AddDays(7)),
                "Title 10", "Description 10", 30, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("cf97c7f6-32cc-4e86-ab5d-bf48039f097b"), new DateAndTime(Now.AddDays(20)),
                "Title 11", "Description 11", 25, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("6e96bb1e-3089-4817-9019-7da90af741ca"), new DateAndTime(Now.AddDays(30)),
                "Title 12", "Description 12", 45, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("ea5b698e-3c36-40a1-9ed5-c56c80119285"), new DateAndTime(Now.AddDays(50)),
                "Title 13", "Description 13", 45, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("2bc34723-bab3-47ae-aa6a-fb1fb1c76d67"), new DateAndTime(Now.AddDays(60)),
                "Title 14", "Description 14", 45, new DateAndTime(Now)),
            ToDoTask.Create(Guid.Parse("4a603cd8-0b68-4ea7-90cc-22a2b6c327d4"), new DateAndTime(Now.AddDays(70)),
                "Title 15", "Description 15", 45, new DateAndTime(Now)),
        };

        context.ToDoTasks.AddRange(toDoTasks);
        await context.SaveChangesAsync();
        return context;
    }
}