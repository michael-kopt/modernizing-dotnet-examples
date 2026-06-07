namespace OAuthMigrationSample.Services;

public sealed class LegacyGoogleRedirectUriBuilder
{
    public string BuildLegacyCallbackUri(HttpRequest request)
    {
        return $"{request.Scheme}://{request.Host}/auth/google/callback";
    }
}
