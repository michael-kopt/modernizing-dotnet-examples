namespace AuditPatternSample.Models;

public sealed class AuditContext
{
    public AuditContext(string methodName)
    {
        MethodName = methodName;
        Status = AuditStatus.Success;
        Timestamp = DateTime.UtcNow;
    }

    public string MethodName { get; set; }

    public AuditStatus Status { get; set; }

    public Exception? Exception { get; set; }

    public string? IpAddress { get; set; }

    public DateTime Timestamp { get; set; }
}
