using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    private const string CorsPolicyName = "ConfiguredOrigins";
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddHttpClient();
        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

        ConfigureCors(services);
        ConfigureAuthentication(services);
        ConfigureDataProtection(services);
        ConfigureSwagger(services);
        ConfigureHealthChecks(services);
        ConfigureMetrics(services);
        ConfigureBackgroundServices(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseCors(CorsPolicyName);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMetricsAllEndpoints();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });
            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });

        if (_configuration.GetValue("Swagger:Enabled", true))
        {
            EnableSwagger(app);
        }
    }

    private void ConfigureCors(IServiceCollection services)
    {
        var allowedOrigins = _configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();
        var allowCredentials = _configuration.GetValue("Cors:AllowCredentials", false);

        services.AddCors(options => options.AddPolicy(CorsPolicyName, policy =>
        {
            if (allowedOrigins.Length == 0)
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                return;
            }

            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            if (allowCredentials)
            {
                policy.AllowCredentials();
            }
        }));
    }
}
