# Part 13: Replacing PostSharp Audit Attribute with Try-Catch-Finally Pattern

Article: [Modernizing .NET - Part 13 Replacing PostSharp Audit Attribute with Try-Catch-Finally Pattern](https://medium.com/@michael.kopt/modernizing-net-part-13-replacing-postsharp-audit-attribute-with-try-catch-finally-pattern-b8832aa170a9)

This sample demonstrates a pragmatic replacement for PostSharp's `[Audit]` attribute in ASP.NET Core. Instead of method interception, the auditing behavior is implemented explicitly with `try`-`catch`-`finally`, which keeps the code dependency-free and easy to debug.

## Sample Focus

- An `AuditStatus` enum with `Success` and `Failure`.
- An `AuditContext` object that captures method name, timestamp, IP address, status, and exception details.
- An `AuditableService` base class that writes audit entries without breaking the main business flow.
- The explicit migration pattern: create audit context, run business logic, update audit state on failure, and always write audit output in `finally`.

## Implementation Notes

- A successful audited operation.
- An early return that still records failure by updating the audit context before returning.
- A background thread that captures request metadata before the thread starts and writes its own audit entry.
- An in-memory audit queue so the behavior is easy to inspect through HTTP endpoints.

## Structure

```text
src/AuditPatternSample
```

## Run

```powershell
dotnet run --project .\src\AuditPatternSample
```

The sample listens on `http://localhost:5881`.

## Try It

Reset audit entries:

```powershell
Invoke-RestMethod -Method Post http://localhost:5881/audit/reset
```

Successful operation:

```powershell
Invoke-RestMethod -Method Post http://localhost:5881/operations/process -ContentType application/json -Body '{"input":"report-42"}'
```

Early return failure:

```powershell
Invoke-RestMethod -Method Post "http://localhost:5881/operations/create-item?authorized=false" -ContentType application/json -Body '{"name":"draft"}'
```

Background thread:

```powershell
Invoke-RestMethod -Method Post http://localhost:5881/operations/background -ContentType application/json -Body '{"input":"sync-users"}'
```

Inspect audit entries:

```powershell
Invoke-RestMethod http://localhost:5881/audit/entries
```

## Notes

- The sample favors explicit method-level auditing over interception so the control flow stays visible and debuggable.
- The background-thread example captures the request IP before thread creation, matching the migration guidance from the article.
