using ToDo.Application.Abstractions;
using ToDo.Core.Abstractions;
using ToDo.Core.Entities;
using ToDo.Core.Repositories;
using ToDo.Core.ValueObjects;

namespace ToDo.Application.Commands.Handlers;

internal sealed class CreateToDoTaskHandler : ICommandHandler<CreateToDoTask>
{
    private readonly IToDoTaskRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateToDoTaskHandler(IToDoTaskRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task HandleAsync(CreateToDoTask command)
    {
        var expirationDate = new DateAndTime(command.ExpirationDate);
        var now = new DateAndTime(_dateTimeProvider.Current());
        
        var toDoTask = ToDoTask.Create(command.ToDoTaskId, expirationDate, command.Title,
            command.Description, command.PercentComplete, now);

        await _repository.CreateAsync(toDoTask);
    }
}