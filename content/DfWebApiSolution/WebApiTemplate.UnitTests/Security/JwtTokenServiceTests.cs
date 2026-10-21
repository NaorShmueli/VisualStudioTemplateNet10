using System.Security.Cryptography;
using BusinessLogic.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Security;
using Moq;

namespace WebApiTemplate.UnitTests.Security;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public async Task CreateAndValidateTokenReturnsTheAuthenticatedSubject()
    {
        using var service = CreateService(CreateOptions());

        var token = service.CreateAccessToken(new CreateAccessTokenRequest
        {
            Subject = "user-42",
            Roles = ["admin"],
            AdditionalClaims = new Dictionary<string, string>
            {
                ["tenant_id"] = "tenant-7"
            }
        });

        var result = await service.ValidateAccessTokenAsync(token.AccessToken);

        Assert.True(result.IsValid);
        Assert.Equal("user-42", result.Subject);
        Assert.NotNull(result.ExpiresAt);
    }

    [Fact]
    public async Task ValidateTokenWithAnotherSigningKeyIsRejected()
    {
        using var issuer = CreateService(CreateOptions());
        using var validator = CreateService(CreateOptions());
        var token = issuer.CreateAccessToken(new CreateAccessTokenRequest
        {
            Subject = "user-42"
        });

        var result = await validator.ValidateAccessTokenAsync(token.AccessToken);

        Assert.False(result.IsValid);
        Assert.Null(result.Subject);
    }

    [Fact]
    public async Task ValidateTokenWhenExpiredIsRejected()
    {
        var options = CreateOptions(accessTokenLifetimeMinutes: 1);
        var oldTime = new FixedTimeProvider(DateTimeOffset.UtcNow.AddHours(-1));
        using var service = CreateService(options, oldTime);
        var token = service.CreateAccessToken(new CreateAccessTokenRequest
        {
            Subject = "user-42"
        });

        var result = await service.ValidateAccessTokenAsync(token.AccessToken);

        Assert.False(result.IsValid);
    }

    private static JwtTokenService CreateService(
        JwtOptions options,
        TimeProvider? timeProvider = null)
    {
        var logger = new Mock<ILogger<JwtTokenService>>();
        return new JwtTokenService(
            Options.Create(options),
            logger.Object,
            timeProvider ?? TimeProvider.System);
    }

    private static JwtOptions CreateOptions(int accessTokenLifetimeMinutes = 5)
    {
        using var rsa = RSA.Create(2048);
        return new JwtOptions
        {
            Enabled = true,
            Issuer = "unit-tests",
            Audience = "unit-test-clients",
            AccessTokenLifetimeMinutes = accessTokenLifetimeMinutes,
            ClockSkewSeconds = 0,
            PrivateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey()),
            PublicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo())
        };
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
