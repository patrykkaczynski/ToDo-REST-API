using ToDo.Application.Abstractions;
using ToDo.Application.Common;
using ToDo.Application.DTO;

namespace ToDo.Application.Queries;

public record GetToDoTasks(string SearchPhrase, int PageNumber, int PageSize, 
    string SortBy, SortDirection SortDirection) : IQuery<PagedResult<ToDoTaskDto>>;