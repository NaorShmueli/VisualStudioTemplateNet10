using WebApiTemplate.BackgroundServices;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    private void ConfigureBackgroundServices(IServiceCollection services)
    {
        var section = _configuration.GetSection(HeartbeatOptions.SectionName);
        var options = section.Get<HeartbeatOptions>() ?? new HeartbeatOptions();

        services.AddOptions<HeartbeatOptions>()
            .Bind(section)
            .Validate(value => value.IntervalSeconds > 0, "Heartbeat interval must be positive.")
            .ValidateOnStart();

        if (options.Enabled)
        {
            services.AddHostedService<HeartbeatBackgroundService>();
        }
    }
}
