using ToDo.Application.Common;
using ToDo.Application.DTO;

namespace ToDo.Infrastructure.DAL.Policies;

internal interface IIncomingFilterPolicy
{
    bool CanBeApplied(IncomingFilter  filter);
    Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasksAsync();
}