using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Repositories;

namespace ToDo.Application.Commands.Handlers;

public class SetToDoPercentCompleteHandler(IToDoTaskRepository repository) : ICommandHandler<SetToDoTaskPercentComplete>
{
    public async Task HandleAsync(SetToDoTaskPercentComplete command)
    {
        var toDoTask = await repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        toDoTask.ChangePercentComplete(command.PercentComplete);
        await repository.UpdateAsync(toDoTask);
    }
}