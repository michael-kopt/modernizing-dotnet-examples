namespace OAuthMigrationSample.Services;

public sealed class FakeOAuthTicketStore
{
    private readonly Dictionary<string, string> stateToCode = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, FakeUserProfile> codeToProfile = new(StringComparer.OrdinalIgnoreCase);
    private readonly object syncRoot = new();

    public string CreateStateToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    public string CreateAuthorizationCode()
    {
        var code = Guid.NewGuid().ToString("N");

        lock (syncRoot)
        {
            codeToProfile[code] = new FakeUserProfile(
                "Google",
                "google-user-123",
                "michael@example.com",
                "Michael Kopt");
        }

        return code;
    }

    public ExchangeResult Exchange(string code, string state)
    {
        lock (syncRoot)
        {
            if (!codeToProfile.TryGetValue(code, out var profile))
            {
                return ExchangeResult.FromFailure("Authorization code is invalid.");
            }

            if (stateToCode.TryGetValue(state, out var existingCode) && !string.Equals(existingCode, code, StringComparison.Ordinal))
            {
                return ExchangeResult.FromFailure("State token does not match the authorization code.");
            }

            stateToCode[state] = code;
            return ExchangeResult.FromSuccess(profile);
        }
    }
}

public sealed record FakeUserProfile(string ProviderName, string ProviderUserId, string Email, string DisplayName);

public sealed record ExchangeResult(bool Success, FakeUserProfile? Profile, string? ErrorMessage)
{
    public static ExchangeResult FromSuccess(FakeUserProfile profile) => new(true, profile, null);

    public static ExchangeResult FromFailure(string errorMessage) => new(false, null, errorMessage);
}
