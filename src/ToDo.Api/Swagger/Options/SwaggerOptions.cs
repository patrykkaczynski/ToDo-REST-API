namespace ToDo.Api.Swagger.Options;

internal class SwaggerOptions
{
    public const string ConfigSection = "Swagger";
    public string DocName { get; set; }
    public string Title { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
}