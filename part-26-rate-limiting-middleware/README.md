# Part 26: Implementing Rate Limiting Middleware in ASP.NET Core

Article: [Modernizing .NET - Part 26 Implementing Rate Limiting Middleware in ASP.NET Core](https://medium.com/@michael.kopt/modernizing-net-part-26-implementing-rate-limiting-middleware-in-asp-net-core-4d8fb87e9930)

This sample is the protected counterpart to Part 25. It takes the same expensive request shape and adds ASP.NET Core rate limiting middleware so concurrency is capped, excess requests wait in a bounded queue, and overflow requests receive a controlled `503 Service Unavailable` response instead of dragging the whole service down.

## Sample Focus

- Global ASP.NET Core concurrency limiting with `AddRateLimiter`.
- Configurable `PermitLimit` and `QueueLimit` through environment variables.
- A protected endpoint that simulates expensive work.
- A burst endpoint that demonstrates processed, queued, and rejected requests.
- A rejection response with `Retry-After` and a clear JSON body.

## Implementation Notes

- `RateLimiterPermitLimit` and `RateLimiterQueueLimit` are read at startup with sane defaults.
- `UseRateLimiter()` is placed before endpoint execution so the expensive work is protected early.
- `/protected/process` performs the same style of mixed CPU and delay work as Part 25.
- `/protected/burst` issues concurrent requests against the protected endpoint and summarizes `200` vs `503` outcomes.

## Structure

```text
src/RateLimitingMiddlewareSample
tests/RateLimitingMiddlewareSample.Tests
```

## Run

```powershell
$env:RateLimiterPermitLimit='4'
$env:RateLimiterQueueLimit='4'
dotnet run --project .\src\RateLimitingMiddlewareSample
```

The sample listens on `http://localhost:5892`.

## Try It

Single protected request:

```powershell
Invoke-RestMethod "http://localhost:5892/protected/process?workMs=80&memoryKb=256"
```

Burst with queueing and rejections:

```powershell
Invoke-RestMethod "http://localhost:5892/protected/burst?requests=20&workMs=80&memoryKb=256"
```

## What To Look For

- `SucceededRequests` stops growing once the configured permit and queue capacity are exhausted.
- `RejectedRequests` becomes non-zero instead of letting active concurrency grow without bound.
- `MaxActiveRequestsObserved` stays close to `PermitLimit`, unlike Part 25 where it could grow to the full burst size.

## Notes

- This sample uses a global limiter because that matches the migration scenario from the article: protect the whole service first, then specialize if needed.
- The right `PermitLimit` still depends on CPU and workload shape. The middleware gives you controlled degradation, not free capacity.
