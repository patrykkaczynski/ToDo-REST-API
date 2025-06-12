using Microsoft.OpenApi.Models;
using ToDo.Api.Swagger.SchemaFilters;

namespace ToDo.Api.Swagger;

public static class Extensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();
            swagger.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "ToDoTask API",
                Version = "v1",
            });
            swagger.SchemaFilter<EnumSchemaFilter>();
        });
        
        services.AddControllers();
        
        return services;
    }
    
    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseReDoc(reDoc =>
        {
            reDoc.RoutePrefix = "docs";
            reDoc.DocumentTitle = "ToDoTask API";
            reDoc.SpecUrl("/swagger/v1/swagger.json");
        });

        return app;
    }
}