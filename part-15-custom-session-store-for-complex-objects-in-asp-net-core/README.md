# Part 15: Custom Session Store for Complex Objects in ASP.NET Core

Article: [Modernizing .NET - Part 15 Custom Session Store for Complex Objects in ASP.NET Core](https://medium.com/@michael.kopt/%EF%B8%8F-custom-session-store-for-complex-objects-in-asp-net-core-1349b680ce12)

This sample demonstrates the migration pattern from the article: keep normal ASP.NET Core session behavior for simple values, but store complex objects by reference in memory so recursive graphs and polymorphic types do not need JSON serialization.

## Sample Focus

- A custom `ISessionStore` that wraps the standard distributed session store and adds an in-memory object dictionary per session.
- Complex session data stored by reference through `IMemoryCache` instead of serialized into JSON.
- A circular object graph that would normally fail with JSON serialization.
- A small legacy-style wrapper that makes the object store easy to consume from application code.

## Implementation Notes

- The sample still uses `AddSession(...)` and the regular `HttpContext.Session` pipeline.
- Primitive session values continue to work through the normal session APIs.
- Complex objects are stored in a per-session object dictionary keyed by the ASP.NET Core session ID.
- The sample keeps the object cache in-process, which matches the article's intended migration use case for temporary workflow state.

## Structure

```text
src/CustomSessionStoreSample
```

## Run

```powershell
dotnet run --project .\src\CustomSessionStoreSample
```

The sample listens on `http://localhost:5883`.

## Try It

Use one PowerShell session object so the cookie stays the same:

```powershell
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
```

Show why plain JSON fails for the object graph:

```powershell
Invoke-RestMethod http://localhost:5883/session/json-attempt
```

Store the workflow state in the custom session store:

```powershell
Invoke-RestMethod -Method Post http://localhost:5883/session/workflow/store -WebSession $session
```

Read it back from the same session:

```powershell
Invoke-RestMethod http://localhost:5883/session/workflow/current -WebSession $session
```

Clear the complex object:

```powershell
Invoke-RestMethod -Method Post http://localhost:5883/session/workflow/clear -WebSession $session
```

## Notes

- This pattern is process-local because the object graph lives in `IMemoryCache`, not a distributed cache.
- It is useful for backward-compatible workflow state and temporary reports, but it should not be used as a cross-server session mechanism.
