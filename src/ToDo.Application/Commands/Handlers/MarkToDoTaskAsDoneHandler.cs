using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Repositories;

namespace ToDo.Application.Commands.Handlers;

internal sealed class MarkToDoTaskAsDoneHandler(IToDoTaskRepository repository) : ICommandHandler<MarkToDoTaskAsDone>
{
    public async Task HandleAsync(MarkToDoTaskAsDone command)
    {
        var toDoTask = await repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        toDoTask.MarkAsDone();
        await repository.UpdateAsync(toDoTask);
    }
}