using Abstraction.Security;
using BusinessLogic.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Security;

namespace WebApiTemplate.AppStart;

public partial class Startup
{
    private void ConfigureAuthentication(IServiceCollection services)
    {
        var jwtSection = _configuration.GetSection(JwtOptions.SectionName);
        var jwt = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
        var googleEnabled = _configuration.GetValue("Authentication:Google:Enabled", false);

        services.AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .Validate(
                options => !options.Enabled ||
                    (!string.IsNullOrWhiteSpace(options.Issuer) &&
                     !string.IsNullOrWhiteSpace(options.Audience) &&
                     !string.IsNullOrWhiteSpace(options.PrivateKey) &&
                     !string.IsNullOrWhiteSpace(options.PublicKey) &&
                     options.AccessTokenLifetimeMinutes > 0 &&
                     options.ClockSkewSeconds >= 0),
                "Enabled JWT requires issuer, audience, RSA keys, and positive lifetime values.")
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        var authentication = services.AddAuthentication(options =>
        {
            if (jwt.Enabled)
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            else if (googleEnabled)
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Google";
            }
        });

        if (jwt.Enabled)
        {
            authentication.AddJwtBearer(options =>
            {
                var rsa = RsaKeyLoader.LoadPublicKey(jwt.PublicKey!);
                options.MapInboundClaims = false;
                options.TokenValidationParameters = JwtTokenService.CreateValidationParameters(
                    jwt,
                    new RsaSecurityKey(rsa));
            });
        }

        if (googleEnabled)
        {
            authentication
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddGoogle("Google", options =>
                {
                    options.ClientId = _configuration["Authentication:Google:ClientId"]
                        ?? throw new OptionsValidationException(
                            "Authentication:Google",
                            typeof(string),
                            ["Google ClientId is required when Google authentication is enabled."]);
                    options.ClientSecret = _configuration["Authentication:Google:ClientSecret"]
                        ?? throw new OptionsValidationException(
                            "Authentication:Google",
                            typeof(string),
                            ["Google ClientSecret is required when Google authentication is enabled."]);
                    options.CallbackPath = _configuration["Authentication:Google:CallbackPath"]
                        ?? "/signin-google";
                    options.SaveTokens = true;
                });
        }

        services.AddAuthorization();
    }
}
