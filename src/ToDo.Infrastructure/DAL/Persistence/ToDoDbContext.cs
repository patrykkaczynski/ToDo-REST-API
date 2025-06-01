using Microsoft.EntityFrameworkCore;
using ToDo.Core.Entities;

namespace ToDo.Infrastructure.DAL.Persistence;

internal sealed class ToDoDbContext(DbContextOptions<ToDoDbContext> options) : DbContext(options)
{
    public DbSet<ToDoTask> ToDoTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}