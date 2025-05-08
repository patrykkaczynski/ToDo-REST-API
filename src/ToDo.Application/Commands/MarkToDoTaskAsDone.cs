using ToDo.Application.Abstractions;

namespace ToDo.Application.Commands;

public record MarkToDoTaskAsDone(Guid ToDoTaskId) : ICommand;