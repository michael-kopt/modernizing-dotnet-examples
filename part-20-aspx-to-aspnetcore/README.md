# Part 20: Migrating Legacy ASPX Pages to ASP.NET Core

Article: [Modernizing .NET - Part 20 Migrating Legacy ASPX Pages to ASP.NET Core](https://medium.com/@michael.kopt)

This sample demonstrates the hybrid migration pattern from the article: keep a legacy `.aspx` route for compatibility, serve a static HTML shell for the UI, and handle postback-style interactions through ASP.NET Core endpoints that return JSON or files.

## Sample Focus

- A static HTML page that replaces WebForms markup with plain HTML and JavaScript.
- Hidden `__EVENTTARGET` and `__EVENTARGUMENT` fields to preserve the old postback contract.
- A backend endpoint that branches on `__EVENTTARGET`.
- Initial data loading with `GetData`.
- Query-parameter support for a `lite` page variation.
- File download behavior from a postback-style action.
- An nginx route example that serves static HTML for GET and proxies POST to ASP.NET Core.

## Implementation Notes

- The sample uses a plain GET route at `/Page.aspx` to return the static HTML shell directly from ASP.NET Core for local verification.
- The POST route at `/Page.aspx` behaves like the old page lifecycle entry point.
- JavaScript handles initial page loading, data binding, postback calls, and download actions.
- The backend logic is isolated in a service class so it resembles migrated code-behind logic rather than controller-only branching.
- A production-style nginx config fragment is included under `nginx/` to show the split-routing trick from the article.

## Structure

```text
src/AspxMigrationSample
nginx/page.conf
```

## Run

```powershell
dotnet run --project .\src\AspxMigrationSample
```

The sample listens on `http://localhost:5886`.

## Try It

Open the page shell:

```powershell
Invoke-WebRequest http://localhost:5886/Page.aspx
```

Request initial data directly:

```powershell
Invoke-RestMethod -Method Post http://localhost:5886/Page.aspx -Body @{ __EVENTTARGET = 'GetData' }
```

Request the lite version:

```powershell
Invoke-RestMethod -Method Post "http://localhost:5886/Page.aspx?lite=true" -Body @{ __EVENTTARGET = 'GetData' }
```

Trigger the download action:

```powershell
Invoke-WebRequest -Method Post http://localhost:5886/Page.aspx -Body @{ __EVENTTARGET = 'GetPage' } -OutFile .\PageExport.zip
```

## Notes

- In production, nginx or another reverse proxy can split GET and POST handling exactly as described in the article. For the sample, both behaviors are hosted inside one app for simplicity, and the equivalent nginx rule is included in `nginx/page.conf`.
- The goal is compatibility-first modernization, not a full SPA rewrite.
