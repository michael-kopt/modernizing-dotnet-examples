namespace AuditPatternSample.Services;

public sealed class BusinessOperationStore
{
    private readonly List<string> items = [];
    private readonly object syncRoot = new();

    public void Add(string item)
    {
        lock (syncRoot)
        {
            items.Add(item);
        }
    }

    public IReadOnlyList<string> GetAll()
    {
        lock (syncRoot)
        {
            return items.ToArray();
        }
    }

    public void Clear()
    {
        lock (syncRoot)
        {
            items.Clear();
        }
    }
}
