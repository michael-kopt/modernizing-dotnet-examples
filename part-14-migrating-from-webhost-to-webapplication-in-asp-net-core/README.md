# Part 14: Migrating from WebHost to WebApplication in ASP.NET Core

Article: [Modernizing .NET - Part 14 Migrating from WebHost to WebApplication in ASP.NET Core](https://medium.com/@michael.kopt/modernizing-net-part-14-migrating-from-webhost-to-webapplication-in-asp-net-core-612a7a8e1b88)

This sample demonstrates the internal ASP.NET Core modernization described in the article: moving from the older `WebHost` startup model to the newer `WebApplication` hosting model introduced in .NET 6.

## Sample Focus

- Replace `WebHost.CreateDefaultBuilder(args)` with `WebApplication.CreateBuilder(args)`.
- Move service registration from `.ConfigureServices(...)` to `builder.Services`.
- Move middleware from `.Configure(...)` into direct calls on `app`.
- Replace `.UseEndpoints(endpoints => endpoints.MapControllers())` with `app.MapControllers()`.
- Access the service provider through `app.Services` only after `builder.Build()`.

## Implementation Notes

- A controller-based app using the modern linear hosting flow.
- Conditional middleware with `UseWhen(...)` for `/api` routes.
- Session configuration in `builder.Services.AddSession(...)`.
- Safe startup initialization after build through `app.Services`.
- Small endpoints that make the migration effects visible instead of only showing startup code.

## Before vs After

Legacy `WebHost` shape:

```csharp
WebHost.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddControllers();
        services.AddSession();
    })
    .Configure(app =>
    {
        app.UseSession();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    })
    .Build()
    .Run();
```

Migrated `WebApplication` shape:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSession();

var app = builder.Build();
app.UseSession();
app.MapControllers();
app.Run();
```

## Structure

```text
src/WebApplicationMigrationSample
```

## Run

```powershell
dotnet run --project .\src\WebApplicationMigrationSample
```

The sample listens on `http://localhost:5882`.

## Try It

API request through conditional middleware:

```powershell
Invoke-RestMethod http://localhost:5882/api/ping
```

Session-backed visit counter:

```powershell
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
Invoke-RestMethod http://localhost:5882/session/visit -WebSession $session
Invoke-RestMethod http://localhost:5882/session/visit -WebSession $session
```

Startup marker resolved from `app.Services` after build:

```powershell
Invoke-RestMethod http://localhost:5882/startup/state
```

## Notes

- The sample keeps the focus on hosting-model changes rather than framework version migration.
- `UseWhen(...)` and session configuration are included because they become easier to reason about in the linear `WebApplication` startup flow.
