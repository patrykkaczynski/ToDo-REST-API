using ToDo.Application.Abstractions;

namespace ToDo.Application.Commands;

public record SetToDoTaskPercentComplete(Guid ToDoTaskId, int PercentComplete) : ICommand;