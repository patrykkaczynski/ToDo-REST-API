using ToDo.Application;
using ToDo.Core;
using ToDo.Infrastructure;
using ToDo.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddControllers();

builder.UseSerilog();

var app = builder.Build();

app.UseInfrastructure();

app.MapControllers();

app.Run();