using ToDo.Application.Abstractions;
using ToDo.Application.Exceptions;
using ToDo.Core.Abstractions;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Commands.Handlers;

public class UpdateToDoTaskHandler(IToDoTaskRepository repository, IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateToDoTask>
{
    public async Task HandleAsync(UpdateToDoTask command)
    {
        var toDoTask = await repository.GetByIdAsync(command.ToDoTaskId);
        if (toDoTask is null)
        {
            throw new ToDoTaskNotFoundException(command.ToDoTaskId);
        }

        var expirationDate = new DateAndTime(command.ExpirationDate);
        var now = new DateAndTime(dateTimeProvider.Current());
        toDoTask.Update(expirationDate, command.Title,command.Description, command.PercentComplete, now);
        await repository.UpdateAsync(toDoTask);
    }
}