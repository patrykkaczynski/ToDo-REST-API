using ToDo.Application.Abstractions;

namespace ToDo.Application.Commands;

public record DeleteToDoTask(Guid ToDoTaskId) : ICommand;
