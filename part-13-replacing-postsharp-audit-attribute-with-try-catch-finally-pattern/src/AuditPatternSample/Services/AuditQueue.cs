using AuditPatternSample.Models;

namespace AuditPatternSample.Services;

public sealed class AuditQueue
{
    private readonly List<AuditEntry> entries = [];
    private readonly object syncRoot = new();

    public void Enqueue(AuditEntry entry)
    {
        lock (syncRoot)
        {
            entries.Add(entry);
        }
    }

    public IReadOnlyList<AuditEntry> GetAll()
    {
        lock (syncRoot)
        {
            return entries.ToArray();
        }
    }

    public void Clear()
    {
        lock (syncRoot)
        {
            entries.Clear();
        }
    }
}
