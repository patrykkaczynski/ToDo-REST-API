using Microsoft.EntityFrameworkCore;
using ToDo.Application.DTO;
using ToDo.Application.Enums;
using ToDo.Core.Abstractions;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Policies;

internal sealed class TodayIncomingFilterPolicy : IIncomingFilterPolicy
{
    private readonly DbSet<ToDoTask> _toDoTasks;
    private readonly IDateTimeProvider _dateTimeProvider;
    
    public TodayIncomingFilterPolicy(ToDoDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _toDoTasks = dbContext.ToDoTasks;
        _dateTimeProvider = dateTimeProvider;
    }

    public bool CanBeApplied(IncomingFilter incomingFilter)
        => incomingFilter == IncomingFilter.Today;

    public async Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasksAsync()
    {
        var now  = _dateTimeProvider.Current();
        var today = new DateAndTime(now);
        var tomorrow = new DateAndTime(now).AddDays(1);
    
        return await _toDoTasks
            .Where(x => x.ExpirationDate >= today && x.ExpirationDate < tomorrow)
            .Select(x => x.AsDto())
            .AsNoTracking()
            .ToListAsync();
    }
}