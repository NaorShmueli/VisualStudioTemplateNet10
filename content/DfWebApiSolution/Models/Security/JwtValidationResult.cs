namespace Models.Security;

public sealed record JwtValidationResult(
    bool IsValid,
    string? Subject,
    DateTimeOffset? ExpiresAt,
    string? Error)
{
    public static JwtValidationResult Invalid(string error) => new(false, null, null, error);
}
