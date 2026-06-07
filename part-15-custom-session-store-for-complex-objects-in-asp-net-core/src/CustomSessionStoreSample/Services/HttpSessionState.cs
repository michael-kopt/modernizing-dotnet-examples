using Microsoft.AspNetCore.Http;

namespace CustomSessionStoreSample.Services;

public sealed class HttpSessionState(ISession session, SessionStore sessionStore)
{
    public object? this[string key]
    {
        get => sessionStore.GetObject(session, key);
        set => sessionStore.SetObject(session, key, value);
    }
}
