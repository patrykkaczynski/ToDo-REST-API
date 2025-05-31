using ToDo.Application.Abstractions;
using ToDo.Application.Common;
using ToDo.Application.DTO;

namespace ToDo.Application.Queries;

public record GetIncomingToDoTasks(IncomingFilter IncomingFilter) : IQuery<IEnumerable<ToDoTaskDto>>;