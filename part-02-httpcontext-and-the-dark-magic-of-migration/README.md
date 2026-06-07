# Part 02: HttpContext and the Dark Magic of Migration

Article: [Modernizing .NET - Part 2: HttpContext and the Dark Magic of Migration](https://medium.com/@michael.kopt/%EF%B8%8F-modernizing-net-part-2-httpcontext-and-the-dark-magic-of-migration-621b0ce7586c)

This sample demonstrates the compatibility shim from the article: recreating `System.Web.HttpContext.Current` in `.NET 8` so legacy code can continue to access request state without immediate DI refactoring.

## Key Idea

The article's core pattern is:

```csharp
namespace System.Web
{
    public static class HttpContext
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get { return _httpContextAccessor.HttpContext; }
        }
    }
}
```

And in application startup:

```csharp
services.AddHttpContextAccessor();
System.Web.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
```

## Project Layout

- `src/HttpContextShimSample` is a minimal ASP.NET Core app
- `Legacy/LegacyRequestReader.cs` simulates old code that accesses `System.Web.HttpContext.Current`
- `SystemWeb/HttpContext.cs` contains the compatibility shim

## What The Sample Shows

- a request handled through a legacy-style static access pattern
- request data read without injecting `IHttpContextAccessor` into the legacy class
- the same access pattern working after an `await`

## Run

From this folder:

```powershell
dotnet run --project .\src\HttpContextShimSample\HttpContextShimSample.csproj
```

Then open:

- `http://localhost:5090/legacy`
- `http://localhost:5090/legacy/async`

Both endpoints return request details collected through `System.Web.HttpContext.Current`.

## Caveat

This is a migration bridge, not a long-term design target. It helps keep large legacy codebases moving while you gradually replace static request access with proper dependency injection.
