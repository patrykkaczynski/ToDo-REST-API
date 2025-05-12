using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Repositories;

namespace ToDo.Application.Commands.Handlers;

internal sealed class MarkToDoTaskAsDoneHandler : ICommandHandler<MarkToDoTaskAsDone>
{
    private readonly IToDoTaskRepository _repository;

    public MarkToDoTaskAsDoneHandler(IToDoTaskRepository repository)
        => _repository = repository;

    public async Task HandleAsync(MarkToDoTaskAsDone command)
    {
        var toDoTask = await _repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        toDoTask.MarkAsDone();
        await _repository.UpdateAsync(toDoTask);
    }
}