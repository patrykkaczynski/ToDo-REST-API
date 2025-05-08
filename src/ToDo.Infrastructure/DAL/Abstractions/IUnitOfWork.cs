namespace ToDo.Infrastructure.DAL.Abstractions;

internal interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
}