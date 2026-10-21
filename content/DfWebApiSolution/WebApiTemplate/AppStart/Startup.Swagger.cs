using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    private const string ApiVersion = "v1";

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc(ApiVersion, new OpenApiInfo
            {
                Title = _configuration["Swagger:Title"] ?? "WebApiTemplate API",
                Version = ApiVersion,
                Description = _configuration["Swagger:Description"]
                    ?? "A layered ASP.NET Core Web API."
            });

            var xmlPath = Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Enter a JWT bearer token."
            });
        });
    }

    private static void EnableSwagger(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";
            options.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", "WebApiTemplate API v1");
        });
    }
}
