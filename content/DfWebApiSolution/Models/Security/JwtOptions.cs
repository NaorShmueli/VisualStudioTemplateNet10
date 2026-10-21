namespace Models.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    public bool Enabled { get; init; }

    public string Issuer { get; init; } = "WebApiTemplate";

    public string Audience { get; init; } = "WebApiTemplate.Clients";

    public int AccessTokenLifetimeMinutes { get; init; } = 15;

    public int ClockSkewSeconds { get; init; } = 30;

    public string? PrivateKey { get; init; }

    public string? PublicKey { get; init; }
}
