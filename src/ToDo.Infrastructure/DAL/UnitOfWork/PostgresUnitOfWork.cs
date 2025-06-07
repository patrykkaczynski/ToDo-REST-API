using ToDo.Infrastructure.DAL.Abstractions;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.UnitOfWork;

internal sealed class PostgresUnitOfWork(ToDoDbContext dbContext) : IUnitOfWork
{
    public async Task ExecuteAsync(Func<Task> action)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await action();
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}