namespace StranglerFigSample.Services;

public sealed class LegacyExportHandler
{
    public Task<LegacyExportResult> HandleAsync(string reportId, CancellationToken cancellationToken)
    {
        return Task.FromResult(new LegacyExportResult(
            Source: "legacy",
            ReportId: reportId,
            FileName: $"{reportId}.csv",
            Notes: "Still routed to the old implementation."));
    }
}

public sealed class ModernOrdersHandler
{
    public Task<ModernOrderResult> HandleAsync(int orderId, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ModernOrderResult(
            Source: "modern",
            OrderId: orderId,
            Status: "ready",
            Notes: "Native ASP.NET Core endpoint with no legacy fallback."));
    }
}

public sealed record LegacyExportResult(
    string Source,
    string ReportId,
    string FileName,
    string Notes);

public sealed record ModernOrderResult(
    string Source,
    int OrderId,
    string Status,
    string Notes);
