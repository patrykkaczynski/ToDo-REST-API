using Microsoft.EntityFrameworkCore;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Repositories;

internal sealed class ToDoTaskRepository(ToDoDbContext dbContext) : IToDoTaskRepository
{
    private readonly DbSet<ToDoTask> _toDoTasks = dbContext.ToDoTasks;

    public Task<ToDoTask> GetByIdAsync(ToDoTaskId id)
    {
        return _toDoTasks.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateAsync(ToDoTask toDoTask)
    {
        await _toDoTasks.AddAsync(toDoTask);
    }

    public Task UpdateAsync(ToDoTask toDoTask)
    {
        _toDoTasks.Update(toDoTask);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ToDoTask toDoTask)
    {
        _toDoTasks.Remove(toDoTask);
        return Task.CompletedTask;
    }
}