using ToDo.Application.Abstractions;
using ToDo.Application.DTO;
using ToDo.Application.Enums;

namespace ToDo.Application.Queries;

public record GetIncomingToDoTasks(IncomingFilter incomingFilter) : IQuery<IEnumerable<ToDoTaskDto>>;