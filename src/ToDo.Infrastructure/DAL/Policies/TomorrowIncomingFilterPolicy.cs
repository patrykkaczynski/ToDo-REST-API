using Microsoft.EntityFrameworkCore;
using ToDo.Application.DTO;
using ToDo.Application.Enums;
using ToDo.Core.Entities;
using ToDo.Core.ValueObjects;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.Policies;

internal sealed class TomorrowIncomingFilterPolicy : IIncomingFilterPolicy
{
    private readonly DbSet<ToDoTask> _toDoTasks;

    public TomorrowIncomingFilterPolicy(ToDoDbContext dbContext)
        => _toDoTasks = dbContext.ToDoTasks;

    public bool CanBeApplied(IncomingFilter incomingFilter)
        => incomingFilter == IncomingFilter.Tomorrow;

    public async Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasks()
    {
        var now = DateTime.UtcNow.Date;
        var tomorrow = new DateAndTime(now).AddDays(1);
        var dayAfterTomorrowStart = new DateAndTime(now).AddDays(2);

        return await _toDoTasks
            .Where(x => x.ExpirationDate >= tomorrow && x.ExpirationDate < dayAfterTomorrowStart)
            .Select(x => x.AsDto())
            .AsNoTracking()
            .ToListAsync();
    }
}