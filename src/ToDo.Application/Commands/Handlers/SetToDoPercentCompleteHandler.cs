using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Repositories;

namespace ToDo.Application.Commands.Handlers;

public class SetToDoPercentCompleteHandler : ICommandHandler<SetToDoTaskPercentComplete>
{
    private readonly IToDoTaskRepository _repository;

    public SetToDoPercentCompleteHandler(IToDoTaskRepository repository)
        => _repository = repository;

    public async Task HandleAsync(SetToDoTaskPercentComplete command)
    {
        var toDoTask = await _repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        toDoTask.ChangePercentComplete(command.PercentComplete);
        await _repository.UpdateAsync(toDoTask);
    }
}