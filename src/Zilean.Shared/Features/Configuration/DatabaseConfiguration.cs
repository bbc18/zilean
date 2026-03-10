namespace Zilean.Shared.Features.Configuration;

public class DatabaseConfiguration
{
  public string ConnectionString { get; set; }

  public DatabaseConfiguration()
  {
    var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "zilean";
    var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "zilean-postgres";
    var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "zilean";
    var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "zilean";

    ConnectionString = $"Host={host};Database={db};Username={user};Password={password};Include Error Detail=true;Timeout=30;CommandTimeout=3600;";
  }
}
