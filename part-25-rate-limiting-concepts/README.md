# Part 25: Rate Limiting Concepts and Strategies

Article: [Modernizing .NET - Part 25 Rate Limiting Concepts and Strategies](https://medium.com/@michael.kopt/modernizing-net-part-25-rate-limiting-concepts-and-strategies-9df1a8a1c7e7)

This sample intentionally shows the failure mode before a limiter is added. It runs an unprotected ASP.NET Core endpoint, drives concurrent traffic into it, and returns a summary showing how active request count and response times grow when nothing constrains concurrency.

## Sample Focus

- A deliberately unprotected endpoint that simulates expensive work.
- A self-load endpoint that creates a burst of concurrent requests.
- A summary showing active-request growth, average latency, and P95 latency.
- A concrete demonstration of why "accept everything" is not a stability strategy.

## Implementation Notes

- `/unsafe/process` performs mixed CPU and blocking-style work and records the active request count seen by each request.
- `/unsafe/burst` uses `HttpClient` to call `/unsafe/process` concurrently from inside the sample app.
- The sample does not reject anything yet. That is the point: all requests are accepted and overall latency degrades.
- Part 26 is where the same shape will be protected with a real ASP.NET Core rate limiter.

## Structure

```text
src/RateLimitingFailureSample
tests/RateLimitingFailureSample.Tests
```

## Run

```powershell
dotnet run --project .\src\RateLimitingFailureSample
```

The sample listens on `http://localhost:5891`.

## Try It

Single request:

```powershell
Invoke-RestMethod "http://localhost:5891/unsafe/process?workMs=80&memoryKb=256"
```

Burst without protection:

```powershell
Invoke-RestMethod "http://localhost:5891/unsafe/burst?requests=24&workMs=80&memoryKb=256"
```

The burst response includes:

- `AverageElapsedMs`
- `P95ElapsedMs`
- `MaxElapsedMs`
- `MaxActiveRequestsObserved`

## What To Look For

- `MaxActiveRequestsObserved` climbs close to the full burst size because nothing caps concurrency.
- `P95ElapsedMs` becomes much larger than the single-request time.
- The system still responds, but it gets progressively less predictable as load grows.

## Notes

- This sample is intentionally missing the fix. It is the "before" picture.
- The work simulation is simplified, but the behavior is the same pattern seen in real services under CPU and memory pressure: too many concurrent requests make every request slower.
