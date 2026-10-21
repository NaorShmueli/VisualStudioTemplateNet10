using Serilog;
using Serilog.Formatting.Compact;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    public static void ConfigureBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateBootstrapLogger();
    }

    public static void ConfigureLogger(
        HostBuilderContext context,
        IServiceProvider services,
        LoggerConfiguration configuration)
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId();
    }
}
