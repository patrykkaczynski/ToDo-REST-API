using System.Linq.Expressions;
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

internal sealed class GetToDoTasksHandler(ToDoDbContext dbContext)
    : IQueryHandler<GetToDoTasks, PagedResult<ToDoTaskDto>>
{
    private readonly DbSet<ToDoTask> _toDoTasks = dbContext.ToDoTasks;

    public async Task<PagedResult<ToDoTaskDto>> HandleAsync(GetToDoTasks query)
    {
        if (!PaginationOptions.AllowedPageSizes.Contains(query.PageSize))
        {
            throw new InvalidPageSizeException(query.PageSize);
        }

        var searchPhrase = query.SearchPhrase?.ToLower();
        var baseQuery = _toDoTasks
            .Where(x => searchPhrase == null ||
                        ((string)x.Title).ToLower().Contains(searchPhrase) ||
                        ((string)x.Description).ToLower().Contains(searchPhrase));

        var totalCount = await baseQuery.CountAsync();
        if (totalCount == 0)
        {
            return new PagedResult<ToDoTaskDto>
            {
                Items = [],
                TotalItemsCount = 0,
                TotalPages = 0,
                ItemsFrom = 0,
                ItemsTo = 0
            };
        }

        var totalPages = (int)Math.Ceiling(totalCount * 1.0 / query.PageSize);
        if (query.PageNumber < 1 || query.PageNumber > totalPages)
        {
            throw new InvalidPageNumberException(query.PageNumber, totalPages);
        }

        if (query.SortBy is not null && !PaginationOptions.AllowedSortByColumnNames
                .Any(x => string.Equals(x, query.SortBy, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidSortByColumnNameException();
        }

        if (query.SortBy is not null)
        {
            var columnSelector =
                new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        nameof(ToDoTask.ExpirationDate),
                        (Expression<Func<ToDoTask, DateTimeOffset>>)(t => t.ExpirationDate)
                    },
                    { nameof(ToDoTask.Title), (Expression<Func<ToDoTask, string>>)(t => t.Title) },
                    { nameof(ToDoTask.Description), (Expression<Func<ToDoTask, string>>)(t => t.Description) },
                    { nameof(ToDoTask.PercentComplete), (Expression<Func<ToDoTask, int>>)(t => t.PercentComplete) },
                };

            var selectedColumn = columnSelector[query.SortBy];

            baseQuery = query.SortDirection is SortDirection.Ascending
                ? Queryable.OrderBy(baseQuery, (dynamic)selectedColumn)
                : Queryable.OrderByDescending(baseQuery, (dynamic)selectedColumn);
        }

        var skippedItems = (query.PageNumber - 1) * query.PageSize;
        var toDoTasks = await baseQuery
            .Skip(skippedItems)
            .Take(query.PageSize)
            .AsNoTracking()
            .Select(t => t.AsDto())
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