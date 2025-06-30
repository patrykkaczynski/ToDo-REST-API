namespace ToDo.Api.Swagger.Options;

internal class ReDocOptions
{
    public const string ConfigSection = "ReDoc";
    public string RoutePrefix { get; set; }
    public string DocumentTitle { get; set; }
    public string SpecUrl { get; set; }
}