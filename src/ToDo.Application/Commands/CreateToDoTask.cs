using ToDo.Application.Abstractions;

namespace ToDo.Application.Commands;

public record CreateToDoTask(
    Guid ToDoTaskId,
    DateTime ExpirationDate,
    string Title,
    string Description,
    int PercentComplete) : ICommand;