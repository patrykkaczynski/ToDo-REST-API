namespace ToDo.Infrastructure.DAL.Options;

internal sealed class PostgresOptions
{
    public const string ConfigSection = "Postgres";
    public string ConnectionString { get; set; }
}