# Part 23: Migrating log4net to Modern .NET Logging

Article: [Modernizing .NET - Part 23 Migrating log4net to Modern .NET Logging](https://medium.com/@michael.kopt/modernizing-net-part-23-migrating-log4net-to-modern-net-logging-0aefab8f0c61)

This sample demonstrates the actual migration point from the article: replace thread-affine log4net context APIs with `LogicalThreadContext`, set request metadata explicitly, and preserve context when work leaves the normal async flow.

## Sample Focus

- `LogicalThreadContext` instead of `NDC` and `ThreadContext`.
- Explicit `SessionId` and `EventTime` properties instead of `%cookie{...}`.
- A disposable request-scope helper that sets and clears log4net context safely.
- Context flow through `await`.
- Explicit capture and restore for work queued with `ThreadPool.UnsafeQueueUserWorkItem`.
- An in-memory appender so the migrated log output is easy to inspect.

## Implementation Notes

- `LoggingScope.Begin(...)` pushes a logical NDC value and sets `UserId`, `TenantId`, `SessionId`, `OperationId`, and `EventTime`.
- `LogContextSnapshot` captures logical properties so they can be restored in background work where execution context does not flow automatically.
- `InMemoryAppender` captures rendered log events for the demo endpoint and tests.
- `log4net.config` shows the updated `%X{...}` pattern that replaces old cookie-based access.

## Structure

```text
src/Log4NetMigrationSample
tests/Log4NetMigrationSample.Tests
```

## Run

```powershell
dotnet run --project .\src\Log4NetMigrationSample
```

The sample listens on `http://localhost:5889`.

## Try It

Show request context flowing across `await`:

```powershell
Invoke-RestMethod "http://localhost:5889/demo?userId=jane.doe&tenantId=tenant-42&sessionId=sess-123&operationId=op-9001"
```

Show explicit capture and restore for background work:

```powershell
Invoke-RestMethod "http://localhost:5889/background?userId=jane.doe&tenantId=tenant-42&sessionId=sess-123&operationId=op-9001"
```

## Notes

- The point is not to replace log4net with another logging stack in one step. The sample shows how to make existing log4net context usage correct in modern async code first.
- `UnsafeQueueUserWorkItem` is included because it makes the context-restoration need visible. Normal `await` already flows `AsyncLocal` state.
