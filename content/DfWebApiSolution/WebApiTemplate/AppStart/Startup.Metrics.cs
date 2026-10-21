using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    private static void ConfigureMetrics(IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
        services.AddMetrics(CreateMetrics());
        services.AddMetricsTrackingMiddleware();
        services.AddMetricsEndpoints(options =>
        {
            options.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
        });
    }

    private static IMetricsRoot CreateMetrics()
    {
        return AppMetrics.CreateDefaultBuilder()
            .Configuration.Configure(options =>
            {
                options.Enabled = true;
                options.ReportingEnabled = true;
            })
            .Build();
    }
}
