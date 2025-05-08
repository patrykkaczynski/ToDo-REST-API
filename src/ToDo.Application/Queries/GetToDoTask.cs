using ToDo.Application.Abstractions;
using ToDo.Application.DTO;

namespace ToDo.Application.Queries;

public record GetToDoTask(Guid ToDoTaskId) : IQuery<ToDoTaskDto>;