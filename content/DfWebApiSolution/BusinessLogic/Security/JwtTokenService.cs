using System.Security.Claims;
using System.Security.Cryptography;
using Abstraction.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Models.Security;

namespace BusinessLogic.Security;

public sealed partial class JwtTokenService : IJwtTokenService, IDisposable
{
    private static readonly HashSet<string> ReservedClaims = new(StringComparer.Ordinal)
    {
        JwtRegisteredClaimNames.Iss,
        JwtRegisteredClaimNames.Aud,
        JwtRegisteredClaimNames.Exp,
        JwtRegisteredClaimNames.Nbf,
        JwtRegisteredClaimNames.Iat,
        JwtRegisteredClaimNames.Jti,
        JwtRegisteredClaimNames.Sub
    };

    private readonly JsonWebTokenHandler _handler = new() { MapInboundClaims = false };
    private readonly ILogger<JwtTokenService> _logger;
    private readonly JwtOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly RSA? _privateKey;
    private readonly RSA? _publicKey;
    private readonly SigningCredentials? _signingCredentials;
    private readonly TokenValidationParameters? _validationParameters;

    public JwtTokenService(
        IOptions<JwtOptions> options,
        ILogger<JwtTokenService> logger,
        TimeProvider timeProvider)
    {
        _options = options.Value;
        _logger = logger;
        _timeProvider = timeProvider;

        if (!_options.Enabled)
        {
            return;
        }

        _privateKey = RsaKeyLoader.LoadPrivateKey(_options.PrivateKey!);
        _publicKey = RsaKeyLoader.LoadPublicKey(_options.PublicKey!);
        _signingCredentials = new SigningCredentials(
            new RsaSecurityKey(_privateKey),
            SecurityAlgorithms.RsaSha256);
        _validationParameters = CreateValidationParameters(
            _options,
            new RsaSecurityKey(_publicKey));
    }

    public AccessTokenResponse CreateAccessToken(CreateAccessTokenRequest request)
    {
        EnsureEnabled();
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Subject);

        var now = _timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(_options.AccessTokenLifetimeMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.Subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(
                JwtRegisteredClaimNames.Iat,
                now.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture),
                ClaimValueTypes.Integer64)
        };

        claims.AddRange(request.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.Ordinal)
            .Select(role => new Claim(ClaimTypes.Role, role)));

        foreach (var (type, value) in request.AdditionalClaims)
        {
            if (ReservedClaims.Contains(type))
            {
                throw new ArgumentException(
                    $"The '{type}' claim is managed by the token service.",
                    nameof(request));
            }

            claims.Add(new Claim(type, value));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = _signingCredentials
        };

        var token = _handler.CreateToken(descriptor);
        LogTokenIssued(_logger, request.Subject, expiresAt);

        return new AccessTokenResponse(token, expiresAt);
    }

    public async ValueTask<JwtValidationResult> ValidateAccessTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();

        if (string.IsNullOrWhiteSpace(token))
        {
            return JwtValidationResult.Invalid("Token is required.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        var result = await _handler.ValidateTokenAsync(token, _validationParameters!);

        if (!result.IsValid)
        {
            LogTokenValidationFailed(_logger);
            return JwtValidationResult.Invalid("Token validation failed.");
        }

        var subject = result.ClaimsIdentity?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var validTo = result.SecurityToken?.ValidTo;
        DateTimeOffset? expiresAt = validTo is null
            ? null
            : new DateTimeOffset(DateTime.SpecifyKind(validTo.Value, DateTimeKind.Utc));

        return new JwtValidationResult(true, subject, expiresAt, null);
    }

    public static TokenValidationParameters CreateValidationParameters(
        JwtOptions options,
        SecurityKey signingKey)
    {
        return new TokenValidationParameters
        {
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.Issuer,
            ValidAudience = options.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromSeconds(options.ClockSkewSeconds)
        };
    }

    public void Dispose()
    {
        _privateKey?.Dispose();
        _publicKey?.Dispose();
    }

    private void EnsureEnabled()
    {
        if (!_options.Enabled)
        {
            throw new InvalidOperationException(
                "JWT is disabled. Enable Authentication:Jwt and configure an RSA key pair first.");
        }
    }

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Debug,
        Message = "Issued an access token for subject {Subject} that expires at {ExpiresAt}")]
    private static partial void LogTokenIssued(
        ILogger logger,
        string subject,
        DateTimeOffset expiresAt);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Debug,
        Message = "Access-token validation failed")]
    private static partial void LogTokenValidationFailed(ILogger logger);
}
