using Microsoft.EntityFrameworkCore;
using ToDo.Application.Abstractions;
using ToDo.Application.Common;
using ToDo.Application.DTO;
using ToDo.Application.Queries;
using ToDo.Core.Entities;
using ToDo.Infrastructure.DAL.Options;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.Exceptions;

namespace ToDo.Infrastructure.DAL.Handlers;

internal sealed class GetToDoTasksHandler : IQueryHandler<GetToDoTasks, PagedResult<ToDoTaskDto>>
{
    private readonly DbSet<ToDoTask> _toDoTasks;

    public GetToDoTasksHandler(ToDoDbContext dbContext)
        => _toDoTasks = dbContext.ToDoTasks;

    public async Task<PagedResult<ToDoTaskDto>> HandleAsync(GetToDoTasks query)
    {
        if (!PaginationOptions.AllowedPageSizes.Contains(query.PageSize))
        {
            throw new InvalidPageSizeException(query.PageSize);
        }

        var baseQuery = _toDoTasks
            .Where(x => query.SearchPhrase == null ||
                        (x.Title.Value.Contains(query.SearchPhrase, StringComparison.OrdinalIgnoreCase) ||
                         x.Description.Value.Contains(query.SearchPhrase, StringComparison.OrdinalIgnoreCase)));

        var totalCount = await baseQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount * 1.0 / query.PageSize);
        if (query.PageNumber < 1 || query.PageNumber > totalPages)
        {
            throw new InvalidPageNumberException(query.PageNumber, totalPages);
        }

        var skippedItems = (query.PageNumber - 1) * query.PageSize;
        var toDoTasks = await baseQuery
            .Skip(skippedItems)
            .Take(query.PageSize)
            .OrderBy(x => x.ExpirationDate)
            .AsNoTracking()
            .Select(x => x.AsDto())
            .ToListAsync();

        return new PagedResult<ToDoTaskDto>
        {
            Items = toDoTasks,
            TotalItemsCount = totalCount,
            TotalPages = totalPages,
            ItemsFrom = skippedItems + 1,
            ItemsTo = skippedItems + query.PageSize
        };
    }
}