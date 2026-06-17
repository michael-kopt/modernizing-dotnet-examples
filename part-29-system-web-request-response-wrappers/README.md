# Part 29: Bonus Round with System.Web Request and Response Wrappers

Article: [Modernizing .NET - Part 29: Bonus Round with System.Web Request and Response Wrappers](https://medium.com/@michael.kopt/%EF%B8%8F-bonus-modernizing-net-part-29-bonus-round-with-system-web-request-and-response-wrappers-0abdec1bae8b)

This sample extends the `HttpContext.Current` bridge from Part 2 with pragmatic `System.Web.HttpRequest` and `System.Web.HttpResponse` wrappers. The goal is not to recreate classic ASP.NET perfectly. The goal is to keep legacy code moving while request and response usage are modernized incrementally.

## Sample Focus

- A `System.Web.HttpContext` compatibility shim backed by `IHttpContextAccessor`.
- A request wrapper that exposes `Params`, `RawUrl`, `UserHostAddress`, `InputStream`, and other legacy-style members.
- A response wrapper that exposes `AppendHeader`, `AddHeader`, `WriteAsync`, `Redirect`, and `IsClientConnected`.
- Minimal endpoints that simulate old code paths without injecting ASP.NET Core abstractions everywhere.

## Structure

```text
src/SystemWebWrappersSample
tests/SystemWebWrappersSample.Tests
```

## Run

```powershell
dotnet run --project .\src\SystemWebWrappersSample
```

The sample listens on `http://localhost:5893`.

## Try It

Request wrapper demo:

```powershell
Invoke-RestMethod "http://localhost:5893/legacy/request?id=42&format=json"
```

Response wrapper demo:

```powershell
Invoke-WebRequest "http://localhost:5893/legacy/export?format=csv"
```

The response includes legacy-style headers such as `X-Legacy-Format`.

## Notes

- `Params` is intentionally a compatibility snapshot built from query, form, cookies, and headers.
- The wrappers are migration scaffolding. They should stay isolated and be removed as endpoints become truly ASP.NET Core-native.
