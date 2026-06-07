using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OAuthMigrationSample.Services;

public static class ClaimsPrincipalFactory
{
    public static ClaimsPrincipal Create(FakeUserProfile profile)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, profile.Email),
            new(ClaimTypes.NameIdentifier, profile.ProviderUserId),
            new(ClaimTypes.Name, profile.DisplayName),
            new("provider", profile.ProviderName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
