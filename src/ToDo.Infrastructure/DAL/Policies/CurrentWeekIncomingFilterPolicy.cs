using Microsoft.EntityFrameworkCore;
using ToDo.Application.DTO;
using ToDo.Application.Enums;
using ToDo.Core.Abstractions;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Policies;

internal sealed class CurrentWeekIncomingFilterPolicy : IIncomingFilterPolicy
{
    private readonly DbSet<ToDoTask> _toDoTasks;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CurrentWeekIncomingFilterPolicy(ToDoDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _toDoTasks = dbContext.ToDoTasks;
        _dateTimeProvider = dateTimeProvider;
    }

    public bool CanBeApplied(IncomingFilter incomingFilter)
        => incomingFilter == IncomingFilter.CurrentWeek;

    public async Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasksAsync()
    {
        var now = _dateTimeProvider.Current();
        var dateNow =  now.Date;
        var currentDay = dateNow.DayOfWeek;
        var daysSinceMonday = currentDay is DayOfWeek.Sunday ? 6 : ((int)currentDay - 1);
        var remainingDays = 6 - daysSinceMonday;
        var from = new DateAndTime(dateNow).AddDays(-daysSinceMonday);
        var to = new DateAndTime(dateNow).AddDays(remainingDays).AddDays(1);

        return await _toDoTasks
            .Where(x => x.ExpirationDate >= from && x.ExpirationDate < to)
            .Select(x => x.AsDto())
            .AsNoTracking()
            .ToListAsync();
    }
}