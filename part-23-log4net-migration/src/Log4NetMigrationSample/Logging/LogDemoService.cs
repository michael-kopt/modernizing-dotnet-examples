using log4net;

namespace Log4NetMigrationSample.Logging;

public sealed class LogDemoService
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LogDemoService));
    private readonly InMemoryAppender _appender;

    public LogDemoService(InMemoryAppender appender)
    {
        _appender = appender;
    }

    public async Task<IReadOnlyList<string>> RunAsyncFlowAsync(CancellationToken cancellationToken)
    {
        Logger.Info("Request started");
        await Task.Delay(25, cancellationToken);
        Logger.Info("After await");

        return _appender.GetEvents();
    }

    public async Task<IReadOnlyList<string>> RunBackgroundFlowAsync()
    {
        Logger.Info("Queueing background work");

        var snapshot = LogContextSnapshot.Capture("UserId", "TenantId", "SessionId", "OperationId", "EventTime");
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            using var restore = snapshot.Restore();
            Logger.Info("Background work item");
            completion.SetResult();
        }, null);

        await completion.Task;
        Logger.Info("Background work complete");

        return _appender.GetEvents();
    }
}
