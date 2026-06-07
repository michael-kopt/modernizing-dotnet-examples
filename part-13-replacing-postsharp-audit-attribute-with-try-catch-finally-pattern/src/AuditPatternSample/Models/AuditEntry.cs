namespace AuditPatternSample.Models;

public sealed class AuditEntry
{
    public string Method { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateTime Timestamp { get; init; }

    public string? IpAddress { get; init; }

    public string? ErrorMessage { get; init; }
}
