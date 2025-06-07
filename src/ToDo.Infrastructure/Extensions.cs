using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ToDo.Core.Abstractions;
using ToDo.Infrastructure.DAL;
using ToDo.Infrastructure.Logging;
using ToDo.Infrastructure.Middlewares;
using ToDo.Infrastructure.Time;

namespace ToDo.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ExceptionMiddleware>();
        services
            .AddPostgres(configuration)
            .AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddCustomLogging();

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }
}