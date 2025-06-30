using ToDo.Api;
using ToDo.Api.Swagger;
using ToDo.Application;
using ToDo.Core;
using ToDo.Infrastructure;
using ToDo.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration)
    .AddControllers();

builder.UseSerilog();

var app = builder.Build();

app.UseInfrastructure();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(app.Configuration);
}

app.MapControllers();

app.Run();

public partial class Program {}