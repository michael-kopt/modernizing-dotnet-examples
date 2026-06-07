# Part 18: OAuth Authentication in ASP.NET Core

Article: [Modernizing .NET - Part 18 OAuth Authentication in ASP.NET Core](https://medium.com/@michael.kopt/modernizing-net-part-18-oauth-authentication-in-asp-net-core-6f77ed506a7d)

This sample demonstrates the migration architecture from the article: fix request scheme detection behind a reverse proxy, preserve a custom callback URL, extract claims into ASP.NET Core authentication, and create a user session. It uses a fully local fake OAuth provider so the flow is runnable without external credentials.

## Sample Focus

- A `RequestSchemeMiddleware` that promotes `X-Forwarded-Proto` and `X-Forwarded-Ssl` into the effective request scheme.
- A callback path at `/auth/google/callback`.
- A redirect builder that mirrors the article's custom handler override pattern by forcing a custom callback URI.
- Claims extraction and cookie-based sign-in after the callback.
- Session state that stores the authenticated user profile after login.

## Implementation Notes

- The sample uses a local fake OAuth provider endpoint instead of Google so the complete flow can be verified offline.
- The challenge endpoint uses a plain route at `/auth/login/google`.
- A cookie authentication scheme is used for the authenticated app session.
- The returned claims include email, name identifier, display name, and provider marker.

## Structure

```text
src/OAuthMigrationSample
```

## Run

```powershell
dotnet run --project .\src\OAuthMigrationSample
```

The sample listens on `http://localhost:5884`.

## Try It

Start the OAuth flow:

```powershell
Invoke-WebRequest http://localhost:5884/auth/login/google -MaximumRedirection 0
```

Simulate reverse-proxy HTTPS detection:

```powershell
Invoke-WebRequest http://localhost:5884/auth/login/google -Headers @{ "X-Forwarded-Proto" = "https" } -MaximumRedirection 0
```

Complete the full login flow with cookie persistence:

```powershell
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
Invoke-WebRequest http://localhost:5884/auth/login/google -WebSession $session | Out-Null
Invoke-RestMethod http://localhost:5884/auth/me -WebSession $session
```

Inspect the stored session profile:

```powershell
Invoke-RestMethod http://localhost:5884/auth/session-profile -WebSession $session
```

## Notes

- The sample demonstrates the redirect and callback architecture, not provider-specific production configuration.
- In a real deployment, `UseForwardedHeaders(...)` is often preferable to custom middleware when the proxy topology is standard.
- The local fake provider is included so the migration pattern is observable without external secrets.
