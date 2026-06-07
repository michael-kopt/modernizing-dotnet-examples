# Part 13: Replacing PostSharp Audit Attribute with Try-Catch-Finally Pattern

This sample demonstrates a pragmatic replacement for PostSharp's `[Audit]` attribute in ASP.NET Core. Instead of method interception, the auditing behavior is implemented explicitly with `try`-`catch`-`finally`, which keeps the code dependency-free and easy to debug.

## Extracted From The Article

- An `AuditStatus` enum with `Success` and `Failure`.
- An `AuditContext` object that captures method name, timestamp, IP address, status, and exception details.
- An `AuditableService` base class that writes audit entries without breaking the main business flow.
- The explicit migration pattern: create audit context, run business logic, update audit state on failure, and always write audit output in `finally`.

## What This Sample Demonstrates

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

## Published Article

[Replacing PostSharp Audit Attributes in ASP.NET Core](https://medium.com/@michael.kopt)
