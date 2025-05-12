using Microsoft.EntityFrameworkCore;
using ToDo.Application.DTO;
using ToDo.Application.Enums;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Policies;

internal sealed class CurrentWeekIncomingFilterPolicy : IIncomingFilterPolicy
{
    private readonly DbSet<ToDoTask> _toDoTasks;

    public CurrentWeekIncomingFilterPolicy(ToDoDbContext dbContext)
        => _toDoTasks = dbContext.ToDoTasks;

    public bool CanBeApplied(IncomingFilter incomingFilter)
        => incomingFilter == IncomingFilter.CurrentWeek;

    public async Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasks()
    {
        var now = DateTime.UtcNow;
        var currentDay = now.Date.DayOfWeek;
        var daysPassed = currentDay is DayOfWeek.Sunday ? 7 : (int)currentDay;
        var remainingDays = 7 - daysPassed;
        var from = new DateAndTime(now).AddDays(-remainingDays);
        var to = new DateAndTime(now).AddDays(remainingDays);

        return await _toDoTasks
            .Where(x => x.ExpirationDate >= from && x.ExpirationDate < to)
            .Select(x => x.AsDto())
            .AsNoTracking()
            .ToListAsync();
    }
}