using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace CustomSessionStoreSample.Services;

public sealed class SessionStore : ISessionStore
{
    private const string SessionCacheKeySuffix = "$session-cache";
    private const string IsSessionNewKey = "$is-new-session";
    private const string SessionTimeoutKey = "$session-timeout";

    private readonly IMemoryCache memoryCache;
    private readonly ISessionStore distributedSessionStore;
    private readonly ILogger<SessionStore> logger;

    public SessionStore(
        IDistributedCache distributedCache,
        ILoggerFactory loggerFactory,
        IMemoryCache memoryCache)
    {
        distributedSessionStore = new DistributedSessionStore(distributedCache, loggerFactory);
        this.memoryCache = memoryCache;
        logger = loggerFactory.CreateLogger<SessionStore>();
    }

    public ISession Create(
        string sessionKey,
        TimeSpan idleTimeout,
        TimeSpan ioTimeout,
        Func<bool> tryEstablishSession,
        bool isNewSessionKey)
    {
        var session = distributedSessionStore.Create(
            sessionKey,
            idleTimeout,
            ioTimeout,
            tryEstablishSession,
            isNewSessionKey);

        var objectStore = GetOrCreateObjectStore(session.Id, idleTimeout, isNewSessionKey);

        if (isNewSessionKey)
        {
            logger.LogInformation("New session created: {SessionId}", session.Id);
        }

        objectStore[SessionTimeoutKey] = (int)idleTimeout.TotalMinutes;
        objectStore[IsSessionNewKey] = isNewSessionKey;

        return session;
    }

    public void SetObject(ISession session, string key, object? value)
    {
        var objectStore = GetObjectStore(session.Id);

        if (objectStore is null)
        {
            return;
        }

        if (value is null)
        {
            objectStore.Remove(key);
            return;
        }

        objectStore[key] = value;
    }

    public object? GetObject(ISession session, string key)
    {
        var objectStore = GetObjectStore(session.Id);

        if (objectStore is null)
        {
            return null;
        }

        return objectStore.TryGetValue(key, out var value) ? value : null;
    }

    public void RemoveObject(ISession session, string key)
    {
        var objectStore = GetObjectStore(session.Id);
        objectStore?.Remove(key);
    }

    public IReadOnlyList<string> GetAllObjectKeys(ISession session)
    {
        var objectStore = GetObjectStore(session.Id);

        return objectStore is null
            ? []
            : objectStore.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public bool GetIsSessionNew(ISession session)
    {
        return GetObject(session, IsSessionNewKey) as bool? ?? false;
    }

    private Dictionary<string, object?> GetOrCreateObjectStore(string sessionId, TimeSpan idleTimeout, bool resetExpiration)
    {
        var cacheKey = GetCacheKey(sessionId);

        if (!memoryCache.TryGetValue(cacheKey, out Dictionary<string, object?>? objectStore) || objectStore is null)
        {
            objectStore = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        }

        if (resetExpiration || !memoryCache.TryGetValue(cacheKey, out _))
        {
            memoryCache.Set(
                cacheKey,
                objectStore,
                new MemoryCacheEntryOptions().SetSlidingExpiration(idleTimeout));
        }

        return objectStore;
    }

    private Dictionary<string, object?>? GetObjectStore(string sessionId)
    {
        return memoryCache.Get<Dictionary<string, object?>>(GetCacheKey(sessionId));
    }

    private static string GetCacheKey(string sessionId)
    {
        return sessionId + SessionCacheKeySuffix;
    }
}
