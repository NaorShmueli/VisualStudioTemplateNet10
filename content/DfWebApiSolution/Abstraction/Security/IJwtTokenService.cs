using Models.Security;

namespace Abstraction.Security;

public interface IJwtTokenService
{
    AccessTokenResponse CreateAccessToken(CreateAccessTokenRequest request);

    ValueTask<JwtValidationResult> ValidateAccessTokenAsync(
        string token,
        CancellationToken cancellationToken = default);
}
