using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    private void ConfigureDataProtection(IServiceCollection services)
    {
        var applicationName = _configuration["DataProtection:ApplicationName"]
            ?? "WebApiTemplate";
        var builder = services.AddDataProtection().SetApplicationName(applicationName);
        var persistToRedis = _configuration.GetValue("DataProtection:PersistKeysToRedis", false);
        var redisConnection = _configuration.GetConnectionString("Redis");

        if (!persistToRedis)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(redisConnection))
        {
            throw new InvalidOperationException(
                "A Redis connection string is required when DataProtection:PersistKeysToRedis is enabled.");
        }

        var redisOptions = ConfigurationOptions.Parse(redisConnection);
        redisOptions.AbortOnConnectFail = false;
        var multiplexer = ConnectionMultiplexer.Connect(redisOptions);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        builder.PersistKeysToStackExchangeRedis(multiplexer, "DataProtection-Keys");
    }
}
