using App.Metrics.AspNetCore;
using Serilog;

namespace WebApiTemplate.AppStart;

public static class Program
{
    public static int Main(string[] args)
    {
        Startup.ConfigureBootstrapLogger();

        try
        {
            Log.Information("Starting WebApiTemplate");
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "WebApiTemplate terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseMetricsWebTracking()
            .UseSerilog((context, services, configuration) =>
                Startup.ConfigureLogger(context, services, configuration))
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
