using ToDo.Application.Abstractions;
using ToDo.Application.DTO;

namespace ToDo.Application.Queries;

public record GetToDoTasks(int PageNumber, int PageSize) : IQuery<IEnumerable<ToDoTaskDto>>;