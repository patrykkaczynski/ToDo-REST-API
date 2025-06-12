using System.Text.Json.Serialization;
using ToDo.Api.Swagger;

namespace ToDo.Api;

public static class Extensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSwaggerDocumentation();
        
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        
        return services;
    }
}