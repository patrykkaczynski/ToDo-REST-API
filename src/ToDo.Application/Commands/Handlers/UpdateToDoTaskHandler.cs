using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Abstractions;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Commands.Handlers;

public class UpdateToDoTaskHandler : ICommandHandler<UpdateToDoTask>
{
    private readonly IToDoTaskRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateToDoTaskHandler(IToDoTaskRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task HandleAsync(UpdateToDoTask command)
    {
        var toDoTask = await _repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        var expirationDate = new DateAndTime(command.ExpirationDate);
        var now = new DateAndTime(_dateTimeProvider.Current());
        toDoTask.Update(expirationDate, command.Title,command.Description, command.PercentComplete, now);
        await _repository.UpdateAsync(toDoTask);
    }
}