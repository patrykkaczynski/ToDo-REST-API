using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Repositories;

namespace ToDo.Application.Commands.Handlers;

internal sealed class DeleteToDoTaskHandler : ICommandHandler<DeleteToDoTask>
{
    private readonly IToDoTaskRepository _repository;

    public DeleteToDoTaskHandler(IToDoTaskRepository repository)
        => _repository = repository;

    public async Task HandleAsync(DeleteToDoTask command)
    {
        var toDoTask = await _repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        await _repository.DeleteAsync(toDoTask);
    }
}