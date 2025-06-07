using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Application.Abstractions;
using ToDo.Core.Repositories;
using ToDo.Infrastructure.DAL.Abstractions;
using ToDo.Infrastructure.DAL.Handlers;
using ToDo.Infrastructure.DAL.Options;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.DAL.Policies;
using ToDo.Infrastructure.DAL.Repositories;
using ToDo.Infrastructure.DAL.UnitOfWork;
using ToDo.Infrastructure.DAL.UnitOfWork.Decorators;

namespace ToDo.Infrastructure.DAL;

internal static class Extensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(PostgresOptions.Postgres);
        services.Configure<PostgresOptions>(section);
        var postgresOptions = section.Get<PostgresOptions>();
        
        services.AddDbContext<ToDoDbContext>(opt 
            => opt.UseNpgsql(postgresOptions.ConnectionString)
                .EnableSensitiveDataLogging());
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        dbContext.Database.Migrate();
        
        services.AddScoped<IToDoTaskRepository, ToDoTaskRepository>();
        services.AddScoped<IUnitOfWork, PostgresUnitOfWork>();
        
        services.TryDecorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));
        
        var infrastructureAssembly = typeof(GetToDoTaskHandler).Assembly;
        
        services.Scan(s => s.FromAssemblies(infrastructureAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IIncomingFilterPolicy)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        services.Scan(s => s.FromAssemblies(infrastructureAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        return services;
    }
}