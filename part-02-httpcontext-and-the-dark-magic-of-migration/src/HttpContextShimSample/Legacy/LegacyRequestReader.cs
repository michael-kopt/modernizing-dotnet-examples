namespace Part02.Legacy;

public static class LegacyRequestReader
{
    public static RequestSnapshot Read()
    {
        var context = System.Web.HttpContext.Current
            ?? throw new InvalidOperationException("HttpContext.Current is not available.");

        return RequestSnapshot.From(context, "sync");
    }

    public static async Task<RequestSnapshot> ReadAfterAwaitAsync()
    {
        await Task.Delay(10);

        var context = System.Web.HttpContext.Current
            ?? throw new InvalidOperationException("HttpContext.Current is not available after await.");

        return RequestSnapshot.From(context, "async");
    }
}
