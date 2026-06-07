# Part 12: Managing Dependency Injection in Background Threads with SharedContext

This sample demonstrates the migration pattern from the article: capture an `IApiClientFactory` during an HTTP request, store it in thread-local state, and reuse it from legacy background entry points that cannot resolve services from ASP.NET Core DI directly.

## What We Can Extract From The Article

- A `SharedContext` bridge that resolves services from request DI when available and falls back to thread-local storage otherwise.
- A small `IApiClientFactory` abstraction over `IHttpClientFactory` so background code can create fresh clients instead of sharing instances.
- Two legacy execution shapes from the article that are easy to demo locally: a raw `Thread` and a `Timer` callback.

## What This Sample Adds

- A minimal ASP.NET Core app with middleware that captures `RequestServices` for the current request.
- Endpoints that trigger background work and record the outcome in memory so the behavior is observable.
- A self-call to `/ping` from both the thread and timer callbacks to prove the same client factory can be reused outside the request pipeline.

## Structure

```text
src/SharedContextSample
```

## Run

```powershell
dotnet run --project .\src\SharedContextSample
```

The sample listens on `http://localhost:5880`.

## Try It

Reset results:

```powershell
Invoke-RestMethod -Method Post http://localhost:5880/work/reset
```

Run the raw thread example:

```powershell
Invoke-RestMethod -Method Post http://localhost:5880/work/request-thread
```

Run the timer example:

```powershell
Invoke-RestMethod -Method Post http://localhost:5880/work/timer
```

Inspect recorded results:

```powershell
Invoke-RestMethod http://localhost:5880/work/results
```

Expected output after both calls:

```json
[
  "thread:pong",
  "timer:pong"
]
```

## Published Article

[Managing Dependency Injection in Background Threads with SharedContext](https://medium.com/@michael.kopt)
