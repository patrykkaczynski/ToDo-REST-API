using ToDo.Application.Abstractions;
using ToDo.Core.Abstractions;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Commands.Handlers;

internal sealed class CreateToDoTaskHandler(IToDoTaskRepository repository, IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateToDoTask>
{
    public async Task HandleAsync(CreateToDoTask command)
    {
        var expirationDate = new DateAndTime(command.ExpirationDate);
        var now = new DateAndTime(dateTimeProvider.Current());
        
        var toDoTask = ToDoTask.Create(command.ToDoTaskId, expirationDate, command.Title,
            command.Description, command.PercentComplete, now);

        await repository.CreateAsync(toDoTask);
    }
}