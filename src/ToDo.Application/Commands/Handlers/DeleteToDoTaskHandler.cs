using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Repositories;

namespace ToDo.Application.Commands.Handlers;

internal sealed class DeleteToDoTaskHandler(IToDoTaskRepository repository) : ICommandHandler<DeleteToDoTask>
{
    public async Task HandleAsync(DeleteToDoTask command)
    {
        var toDoTask = await repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        await repository.DeleteAsync(toDoTask);
    }
}