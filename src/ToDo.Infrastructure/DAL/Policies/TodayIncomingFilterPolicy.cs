using Microsoft.EntityFrameworkCore;
using ToDo.Application.Common;
using ToDo.Application.DTO;
using ToDo.Core.Abstractions;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Policies;

internal sealed class TodayIncomingFilterPolicy(ToDoDbContext dbContext, IDateTimeProvider dateTimeProvider)
    : IIncomingFilterPolicy
{
    private readonly DbSet<ToDoTask> _toDoTasks = dbContext.ToDoTasks;

    public bool CanBeApplied(IncomingFilter incomingFilter)
        => incomingFilter == IncomingFilter.Today;

    public async Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasksAsync()
    {
        var now  = dateTimeProvider.Current();
        var dateNow = now.Date;
        var today = new DateAndTime(dateNow);
        var tomorrow = new DateAndTime(dateNow).AddDays(1);
    
        return await _toDoTasks
            .Where(x => x.ExpirationDate >= today && x.ExpirationDate < tomorrow)
            .Select(x => x.AsDto())
            .AsNoTracking()
            .ToListAsync();
    }
}