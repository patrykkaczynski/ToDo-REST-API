using Microsoft.OpenApi.Models;
using ToDo.Api.Swagger.Options;
using ToDo.Api.Swagger.SchemaFilters;

namespace ToDo.Api.Swagger;

public static class Extensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(swagger =>
        {
            var section = configuration.GetSection(SwaggerOptions.ConfigSection);
            var swaggerOptions = section.Get<SwaggerOptions>();
            
            swagger.EnableAnnotations();
            swagger.SwaggerDoc(swaggerOptions.DocName, new OpenApiInfo()
            {
                Title = swaggerOptions.Title,
                Version = swaggerOptions.Version,
                Description = swaggerOptions.Description,
            });
            swagger.SchemaFilter<EnumSchemaFilter>();
        });
        
        services.AddControllers();
        
        return services;
    }
    
    public static WebApplication UseSwaggerDocumentation(this WebApplication app, IConfiguration configuration)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        var reDocOptions = configuration.GetSection(ReDocOptions.ConfigSection).Get<ReDocOptions>();
        app.UseReDoc(reDoc =>
        {
            reDoc.RoutePrefix = reDocOptions.RoutePrefix ?? "docs";
            reDoc.DocumentTitle = reDocOptions.DocumentTitle ?? "API Documentation";
            reDoc.SpecUrl = reDocOptions.SpecUrl ?? "/swagger/v1/swagger.json";
        });

        return app;
    }
}