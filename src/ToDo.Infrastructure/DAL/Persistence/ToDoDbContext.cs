using Microsoft.EntityFrameworkCore;
using ToDo.Core.Entities;

namespace ToDo.Infrastructure.DAL.Persistence;

internal sealed class ToDoDbContext : DbContext
{
    public DbSet<ToDoTask> ToDoTasks { get; set; }
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}