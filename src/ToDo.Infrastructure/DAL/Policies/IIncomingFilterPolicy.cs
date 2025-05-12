using ToDo.Application.DTO;
using ToDo.Application.Enums;

namespace ToDo.Infrastructure.DAL.Policies;

internal interface IIncomingFilterPolicy
{
    bool CanBeApplied(IncomingFilter  filter);

    Task<IEnumerable<ToDoTaskDto>> GetIncomingToDoTasks();
}