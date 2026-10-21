namespace Models.Security;

public sealed record AccessTokenResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string TokenType = "Bearer");
