using Microsoft.Extensions.DependencyInjection;
using ToDo.Application.Abstractions;

namespace ToDo.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ICommandHandler<>).Assembly;
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        return services;
    }
}