namespace ToDo.Infrastructure.DAL.Options;

internal sealed class PostgresOptions
{
    public const string Postgres = "Postgres";
    
    public string ConnectionString { get; set; }
}