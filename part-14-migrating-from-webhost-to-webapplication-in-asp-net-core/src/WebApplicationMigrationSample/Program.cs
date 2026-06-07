using WebApplicationMigrationSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});
builder.Services.AddSingleton<StartupState>();

var app = builder.Build();

app.UseSession();

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    branch => branch.UseMiddleware<ApiLoggingMiddleware>());

app.MapControllers();

var startupState = app.Services.GetRequiredService<StartupState>();
startupState.MarkInitialized();

app.Run("http://localhost:5882");
