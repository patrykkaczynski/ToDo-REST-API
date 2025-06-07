using ToDo.Application.Abstractions;
using ToDo.Infrastructure.DAL.Abstractions;

namespace ToDo.Infrastructure.DAL.UnitOfWork.Decorators;

internal sealed class UnitOfWorkCommandHandlerDecorator<TCommand>(
    ICommandHandler<TCommand> commandHandler,
    IUnitOfWork unitOfWork)
    : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    public async Task HandleAsync(TCommand command)
    {
        await unitOfWork.ExecuteAsync(() => commandHandler.HandleAsync(command));
    }
}