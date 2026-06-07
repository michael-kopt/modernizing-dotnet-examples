# Part 21: Migrating WebDAV Server Functionality to .NET Core

Article: [Modernizing .NET - Part 21 Migrating WebDAV Server Functionality to .NET Core](https://medium.com/@michael.kopt)

This sample demonstrates the ASP.NET Core side of the migration pattern from the article: expose WebDAV behavior through a controller-like endpoint, support non-standard HTTP verbs, and keep the request-processing logic separate from the routing layer.

## Sample Focus

- A WebDAV endpoint at `/webdav/{*path}` that accepts `PROPFIND`, `PUT`, `GET`, and `OPTIONS`.
- A `DavModule`-style service that returns ASP.NET Core results instead of writing directly to `Response.OutputStream`.
- XML `207 Multi-Status` responses for `PROPFIND`.
- File upload and retrieval through `PUT` and `GET`.
- Header and query handling patterns that differ from .NET Framework.

## Implementation Notes

- The sample uses ASP.NET Core only, but the service structure mirrors the shared-logic split described in the article.
- Uploaded files are stored under a local `Storage/` folder inside the sample project.
- `_MWDRes` query handling is included as a concrete example of request-shape branching from the migration notes.
- `OPTIONS` returns a simple `Allow` header so a WebDAV client can inspect the supported verbs.

## Structure

```text
src/WebDavMigrationSample
```

## Run

```powershell
dotnet run --project .\src\WebDavMigrationSample
```

The sample listens on `http://localhost:5887`.

## Try It

Create or overwrite a file:

```powershell
Invoke-WebRequest -Method Put http://localhost:5887/webdav/docs/report.txt -Body 'hello webdav'
```

Read the file:

```powershell
Invoke-RestMethod http://localhost:5887/webdav/docs/report.txt
```

Request WebDAV properties:

```powershell
Invoke-WebRequest -Method Custom -CustomMethod PROPFIND http://localhost:5887/webdav/docs/report.txt
```

Inspect supported verbs:

```powershell
Invoke-WebRequest -Method Options http://localhost:5887/webdav/docs/report.txt
```

## Notes

- This sample is intentionally minimal and does not attempt full RFC-complete WebDAV behavior.
- The goal is to show how an old `IHttpModule`-style protocol handler maps into ASP.NET Core request handling.
