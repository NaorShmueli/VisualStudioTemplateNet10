using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    private void ConfigureHealthChecks(IServiceCollection services)
    {
        var checks = services.AddHealthChecks();
        var sqlServer = _configuration.GetConnectionString("SqlServer");
        var redis = _configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(sqlServer))
        {
            checks.AddSqlServer(
                sqlServer,
                name: "sql-server",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready", "database", "sql"],
                timeout: TimeSpan.FromSeconds(5));
        }

        if (!string.IsNullOrWhiteSpace(redis))
        {
            checks.AddRedis(
                redis,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: ["ready", "cache", "redis"],
                timeout: TimeSpan.FromSeconds(3));
        }
    }
}
