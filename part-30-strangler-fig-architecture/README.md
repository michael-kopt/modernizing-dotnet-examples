# Part 30: Strangler Fig Architecture

Article: [Modernizing .NET - Part 30: Strangler Fig Architecture](https://medium.com/@michael.kopt/modernizing-net-part-30-strangler-fig-architecture-903da2ce1e9a)

This sample demonstrates the architectural pattern behind the whole migration series: do not rewrite everything at once. Keep the outside contract stable, route one feature at a time to a modern implementation, and keep a fallback path while confidence grows.

## Sample Focus

- A feature router that decides whether a request goes to legacy or modern code.
- Preserving legacy URLs such as `Customer.aspx`.
- A migrated feature handled by modern ASP.NET Core code behind the old route.
- A feature that still goes to the legacy implementation.
- Fallback from modern to legacy when the modern path fails.

## Structure

```text
src/StranglerFigSample
tests/StranglerFigSample.Tests
```

## Run

```powershell
dotnet run --project .\src\StranglerFigSample
```

The sample listens on `http://localhost:5894`.

## Try It

Inspect the current routing map:

```powershell
Invoke-RestMethod http://localhost:5894/feature-map
```

Old URL routed to the modern implementation:

```powershell
Invoke-RestMethod "http://localhost:5894/Customer.aspx?customerId=42"
```

Feature still handled by legacy code:

```powershell
Invoke-RestMethod "http://localhost:5894/Export.ashx?reportId=monthly"
```

Simulate modern failure and automatic fallback:

```powershell
Invoke-RestMethod "http://localhost:5894/Customer.aspx?customerId=42&failModern=true"
```

## Notes

- The point is not the specific business logic. The point is the routing boundary and the coexistence of old and new implementations.
- The sample uses in-process classes for the legacy side, but the same pattern also works when the legacy implementation lives in another app behind a reverse proxy.
