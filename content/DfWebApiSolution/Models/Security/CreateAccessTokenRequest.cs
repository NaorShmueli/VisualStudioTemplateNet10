namespace Models.Security;

public sealed record CreateAccessTokenRequest
{
    public required string Subject { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();

    public IReadOnlyDictionary<string, string> AdditionalClaims { get; init; }
        = new Dictionary<string, string>();
}
