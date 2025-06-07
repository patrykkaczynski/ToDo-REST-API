using Humanizer;
using Microsoft.Extensions.Logging;
using ToDo.Application.Abstractions;
using ToDo.Infrastructure.DAL.Abstractions;

namespace ToDo.Infrastructure.DAL.UnitOfWork.Decorators;

internal sealed class UnitOfWorkCommandHandlerDecorator<TCommand>(
    ICommandHandler<TCommand> commandHandler,
    IUnitOfWork unitOfWork,
    ILogger<UnitOfWorkCommandHandlerDecorator<TCommand>> logger)
    : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    public async Task HandleAsync(TCommand command)
    {
        var commandName = typeof(TCommand).Name.Underscore();;
        logger.LogInformation("Starting unit of work for command {CommandName}: {@Command}", commandName, command);
        await unitOfWork.ExecuteAsync(() => commandHandler.HandleAsync(command));
        logger.LogInformation("Completed unit of work for command {CommandName}: {@Command}", commandName, command);
    }
}