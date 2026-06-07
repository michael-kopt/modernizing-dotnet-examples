# Part 19: Migrating In-Memory Dictionaries to Redis

Article: [Modernizing .NET - Part 19 Migrating In-Memory Dictionaries to Redis](https://medium.com/@michael.kopt/modernizing-net-part-19-migrating-in-memory-dictionaries-to-redis-d8ab37ee354a)

This sample demonstrates the migration path from local `Dictionary` or `ConcurrentDictionary` state to `IDistributedCache` with an optional Redis backend. It keeps the application-level token/session API small while replacing the storage mechanism underneath.

## Sample Focus

- A `DistributedTokenManager` that stores user data and token timestamps behind `IDistributedCache`.
- Key prefixing for token records, user records, and aggregate counters.
- Sliding and absolute expiration through `DistributedCacheEntryOptions`.
- A feature flag that switches between Redis and in-memory distributed cache.
- A small auth API that makes the migration behavior observable.

## Implementation Notes

- The sample uses `Microsoft.Extensions.Caching.StackExchangeRedis` `10.0.8` and `StackExchange.Redis` `2.13.17`.
- `REDIS_ACTIVE=false` is the default so the sample runs without a Redis server.
- When `REDIS_ACTIVE=true`, the sample uses `AddStackExchangeRedisCache(...)` and the connection string from `REDIS_CONNECTION_STRING`.
- The token-count update is intentionally simple and not atomic. In a production Redis-heavy path, you would often use direct Redis primitives for counters.

## Structure

```text
src/DictionaryToRedisSample
```

## Run

Run with the local in-memory fallback:

```powershell
dotnet run --project .\src\DictionaryToRedisSample
```

Run against Redis:

```powershell
$env:REDIS_ACTIVE = "true"
$env:REDIS_CONNECTION_STRING = "localhost:6379"
dotnet run --project .\src\DictionaryToRedisSample
```

The sample listens on `http://localhost:5885`.

## Try It

Create a token:

```powershell
Invoke-RestMethod -Method Post http://localhost:5885/auth/login -ContentType application/json -Body '{"username":"demo","password":"pass123"}'
```

Validate a token:

```powershell
Invoke-RestMethod "http://localhost:5885/auth/validate?token=YOUR_TOKEN"
```

Check the active token count:

```powershell
Invoke-RestMethod http://localhost:5885/auth/token-count
```

Inspect runtime cache mode:

```powershell
Invoke-RestMethod http://localhost:5885/auth/cache-info
```

Logout:

```powershell
Invoke-RestMethod -Method Post "http://localhost:5885/auth/logout?token=YOUR_TOKEN"
```

## Notes

- The fallback mode still uses the distributed-cache abstraction, which keeps the higher-level migration code identical.
- Redis removes the split-brain problem of per-instance dictionaries, but it also introduces a network dependency that needs error handling and monitoring.
