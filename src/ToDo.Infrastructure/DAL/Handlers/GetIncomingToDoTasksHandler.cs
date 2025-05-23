using ToDo.Application.Abstractions;
using ToDo.Application.DTO;
using ToDo.Application.Queries;
using ToDo.Infrastructure.DAL.Policies;
using ToDo.Infrastructure.Exceptions;

namespace ToDo.Infrastructure.DAL.Handlers;

internal sealed class GetIncomingToDoTasksHandler :  IQueryHandler<GetIncomingToDoTasks, IEnumerable<ToDoTaskDto>>
{
    private readonly IEnumerable<IIncomingFilterPolicy> _incomingFiltePolicies;

    public GetIncomingToDoTasksHandler(IEnumerable<IIncomingFilterPolicy> incomingFiltePolicies)
     => _incomingFiltePolicies = incomingFiltePolicies;
    
    public async Task<IEnumerable<ToDoTaskDto>> HandleAsync(GetIncomingToDoTasks query)
    {
        var policy = _incomingFiltePolicies.SingleOrDefault(p => p.CanBeApplied(query.incomingFilter));
        if (policy is null)
        {
            throw new NoIncomingFilterPolicyFound(query.incomingFilter);
        }

        var toDoTasks = await policy.GetIncomingToDoTasks();
        return toDoTasks;
    }
}