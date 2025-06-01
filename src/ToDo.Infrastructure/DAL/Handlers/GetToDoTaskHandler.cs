using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.DTO;
using ToDo.Application.Exceptions;
using ToDo.Application.Queries;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Handlers;

internal sealed class GetToDoTaskHandler(ToDoDbContext dbContext) : IQueryHandler<GetToDoTask, ToDoTaskDto>
{
    private readonly DbSet<ToDoTask> _toDoTasks = dbContext.ToDoTasks;

    public async Task<ToDoTaskDto> HandleAsync(GetToDoTask query)
    {
        var toDoTaskId = new ToDoTaskId(query.ToDoTaskId);
        var toDoTask = await _toDoTasks
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == toDoTaskId);

        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(toDoTaskId);
        }
           
        return toDoTask.AsDto();
    }
}