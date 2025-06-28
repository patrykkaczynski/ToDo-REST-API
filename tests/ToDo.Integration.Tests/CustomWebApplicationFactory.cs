using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Core.Abstractions;
using ToDo.Infrastructure.DAL.Options;
using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Integration.Tests.Time;

namespace ToDo.Integration.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ToDoDbContext>));
            
            if(dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);
            
            var configuration = services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>();
            
            var section = configuration.GetSection(PostgresOptions.Postgres);
            var postgresOptions = section.Get<PostgresOptions>();

            services.AddDbContext<ToDoDbContext>(options =>
            {
                options.UseNpgsql(postgresOptions.ConnectionString);
            });
            
            services.AddScoped<IDateTimeProvider, TestDateTimeProvider>();
        });
        
        builder.UseEnvironment("test");
    }
}