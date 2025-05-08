using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.DTO;
using ToDo.Application.Queries;
using ToDo.Core.Entities;
using ToDo.Infrastructure.DAL.Options;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.Exceptions;

namespace ToDo.Infrastructure.DAL.Handlers;

internal sealed class GetToDoTasksHandler : IQueryHandler<GetToDoTasks, IEnumerable<ToDoTaskDto>>
{
    private readonly DbSet<ToDoTask> _toDoTasks;

    public GetToDoTasksHandler(ToDoDbContext dbContext)
    => _toDoTasks = dbContext.ToDoTasks;

    public async Task<IEnumerable<ToDoTaskDto>> HandleAsync(GetToDoTasks query)
    {
        if (!PaginationOptions.AllowedPageSizes.Contains(query.PageSize))
        {
            throw new InvalidPageSizeException(query.PageSize);
        }
        
        var totalCount = await _toDoTasks.CountAsync();
        var maximalPageNumber = (int)Math.Ceiling(totalCount * 1.0/ query.PageSize);
        if (query.PageNumber < 1 || query.PageNumber > maximalPageNumber)
        {
            throw new InvalidPageNumberException(query.PageNumber, maximalPageNumber);

        }
        
        var skippedItems = (query.PageNumber - 1) * query.PageSize;
        return await _toDoTasks
            .Skip(skippedItems)
            .Take(query.PageSize)
            .OrderBy(x => x.ExpirationDate)
            .AsNoTracking()
            .Select(x => x.AsDto())
            .ToListAsync();
    }
}