using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Repositories;

public interface IToDoTaskRepository
{
    Task<ToDoTask> GetByIdAsync(ToDoTaskId id);
    Task CreateAsync(ToDoTask toDoTask);
    Task UpdateAsync(ToDoTask toDoTask);
    Task DeleteAsync(ToDoTask toDoTask);
}