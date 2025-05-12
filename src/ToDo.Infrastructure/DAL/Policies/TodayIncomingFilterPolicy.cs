using Microsoft.EntityFrameworkCore;
using ToDo.Application.DTO;
using ToDo.Application.Enums;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Policies;

internal sealed class TodayIncomingFilterPolicy : IIncomingFilterPolicy
{
    private readonly DbSet<ToDoTask> _toDoTasks;

    public TodayIncomingFilterPolicy(ToDoDbContext dbContext)
        => _toDoTasks = dbContext.ToDoTasks;

    public bool CanBeApplied(IncomingFilter incomingFilter)
        => incomingFilter == IncomingFilter.Today;

    public async Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasks()
    {
        var now  = DateTime.UtcNow.Date;
        var today = new DateAndTime(now);
        var tomorrow = new DateAndTime(now).AddDays(1);
    
        return await _toDoTasks
            .Where(x => x.ExpirationDate >= today && x.ExpirationDate < tomorrow)
            .Select(x => x.AsDto())
            .AsNoTracking()
            .ToListAsync();
    }
}