using ToDo.Infrastructure.DAL.Abstractions;
using ToDo.Infrastructure.DAL.Persistence;

namespace ToDo.Infrastructure.DAL.UnitOfWork;

internal sealed class PostgresUnitOfWork : IUnitOfWork
{
    private readonly ToDoDbContext _dbContext;

    public PostgresUnitOfWork(ToDoDbContext dbContext)
        => _dbContext = dbContext;

    public async Task ExecuteAsync(Func<Task> action)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await action();
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}