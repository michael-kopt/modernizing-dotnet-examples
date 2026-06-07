using System.Collections.Concurrent;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Log4NetMigrationSample.Logging;

public sealed class InMemoryAppender : AppenderSkeleton
{
    private readonly ConcurrentQueue<string> _events = new();

    protected override void Append(LoggingEvent loggingEvent)
    {
        var userId = loggingEvent.LookupProperty("UserId")?.ToString() ?? string.Empty;
        var tenantId = loggingEvent.LookupProperty("TenantId")?.ToString() ?? string.Empty;
        var sessionId = loggingEvent.LookupProperty("SessionId")?.ToString() ?? string.Empty;
        var operationId = loggingEvent.LookupProperty("OperationId")?.ToString() ?? string.Empty;
        var eventTime = loggingEvent.LookupProperty("EventTime")?.ToString() ?? string.Empty;
        var ndc = loggingEvent.LookupProperty("NDC")?.ToString() ?? string.Empty;

        _events.Enqueue(
            $"{loggingEvent.Level} [EventTime={eventTime}] [{userId},{tenantId},{sessionId},{operationId}] {ndc} - {loggingEvent.RenderedMessage}");
    }

    public void AttachToRootLogger()
    {
        var repository = (Hierarchy)LogManager.GetRepository(typeof(InMemoryAppender).Assembly);
        var rootLogger = repository.Root;

        rootLogger.AddAppender(this);
        ActivateOptions();
    }

    public IReadOnlyList<string> GetEvents()
    {
        return _events.ToArray();
    }

    public void Clear()
    {
        while (_events.TryDequeue(out _))
        {
        }
    }
}
