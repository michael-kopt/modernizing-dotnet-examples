using Log4NetMigrationSample.Logging;
using Xunit;

namespace Log4NetMigrationSample.Tests;

public class LoggingScopeTests
{
    [Fact]
    public async Task LoggingScope_FlowsAcrossAwait()
    {
        using var scope = LoggingScope.Begin("test-user", "tenant-7", "sess-123", "op-456");

        await Task.Delay(10);

        Assert.Equal("test-user", log4net.LogicalThreadContext.Properties["UserId"]?.ToString());
        Assert.Equal("tenant-7", log4net.LogicalThreadContext.Properties["TenantId"]?.ToString());
        Assert.Equal("sess-123", log4net.LogicalThreadContext.Properties["SessionId"]?.ToString());
        Assert.Equal("op-456", log4net.LogicalThreadContext.Properties["OperationId"]?.ToString());
    }

    [Fact]
    public async Task Snapshot_CanRestoreContextInUnsafeQueueUserWorkItem()
    {
        using var scope = LoggingScope.Begin("test-user", "tenant-7", "sess-123", "op-456");
        var snapshot = LogContextSnapshot.Capture("UserId", "TenantId", "SessionId", "OperationId");
        var completion = new TaskCompletionSource<(string? UserId, string? TenantId, string? SessionId, string? OperationId)>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            using var restore = snapshot.Restore();
            completion.SetResult((
                log4net.LogicalThreadContext.Properties["UserId"]?.ToString(),
                log4net.LogicalThreadContext.Properties["TenantId"]?.ToString(),
                log4net.LogicalThreadContext.Properties["SessionId"]?.ToString(),
                log4net.LogicalThreadContext.Properties["OperationId"]?.ToString()));
        }, null);

        var result = await completion.Task;

        Assert.Equal("test-user", result.UserId);
        Assert.Equal("tenant-7", result.TenantId);
        Assert.Equal("sess-123", result.SessionId);
        Assert.Equal("op-456", result.OperationId);
    }
}
