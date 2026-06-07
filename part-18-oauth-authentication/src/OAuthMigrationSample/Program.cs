using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OAuthMigrationSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login.html";
        options.LogoutPath = "/logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddSingleton<LegacyGoogleRedirectUriBuilder>();
builder.Services.AddSingleton<FakeOAuthTicketStore>();

var app = builder.Build();

app.UseSession();
app.UseMiddleware<RequestSchemeMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/login/google", (HttpContext context, LegacyGoogleRedirectUriBuilder redirectBuilder, FakeOAuthTicketStore tickets) =>
{
    var redirectUri = redirectBuilder.BuildLegacyCallbackUri(context.Request);
    var state = tickets.CreateStateToken();

    var authorizeUrl =
        $"{context.Request.Scheme}://{context.Request.Host}/fake-provider/authorize" +
        $"?redirect_uri={Uri.EscapeDataString(redirectUri)}" +
        $"&state={Uri.EscapeDataString(state)}";

    return Results.Redirect(authorizeUrl);
});

app.MapGet("/fake-provider/authorize", (string redirect_uri, string state, FakeOAuthTicketStore tickets) =>
{
    var code = tickets.CreateAuthorizationCode();
    var callbackUrl = $"{redirect_uri}?code={Uri.EscapeDataString(code)}&state={Uri.EscapeDataString(state)}";
    return Results.Redirect(callbackUrl);
});

app.MapGet("/auth/google/callback", async (HttpContext context, string code, string state, FakeOAuthTicketStore tickets) =>
{
    var result = tickets.Exchange(code, state);

    if (!result.Success)
    {
        return Results.Redirect($"/login.html?error=2&error_message={Uri.EscapeDataString(result.ErrorMessage!)}");
    }

    var principal = ClaimsPrincipalFactory.Create(result.Profile!);

    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = "/dashboard"
        });

    context.Session.SetString("UserEmail", result.Profile!.Email);
    context.Session.SetString("UserId", result.Profile.ProviderUserId);
    context.Session.SetString("UserName", result.Profile.DisplayName);

    return Results.Redirect("/dashboard");
});

app.MapGet("/auth/me", async (HttpContext context) =>
{
    var authenticateResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
    {
        return Results.Unauthorized();
    }

    var claims = authenticateResult.Principal.Claims
        .Select(claim => new { claim.Type, claim.Value })
        .ToArray();

    return Results.Ok(new
    {
        authenticated = true,
        claims
    });
});

app.MapGet("/auth/session-profile", (HttpContext context) =>
{
    var email = context.Session.GetString("UserEmail");
    var userId = context.Session.GetString("UserId");
    var userName = context.Session.GetString("UserName");

    if (email is null || userId is null)
    {
        return Results.NotFound(new { message = "No authenticated session profile found." });
    }

    return Results.Ok(new
    {
        email,
        userId,
        userName
    });
});

app.MapGet("/dashboard", (HttpContext context) =>
{
    var email = context.Session.GetString("UserEmail");
    return Results.Ok(new
    {
        message = "OAuth login complete",
        email
    });
});

app.Run("http://localhost:5884");
