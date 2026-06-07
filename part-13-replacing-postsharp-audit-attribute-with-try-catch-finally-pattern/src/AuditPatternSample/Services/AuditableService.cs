using System.Diagnostics;
using System.Runtime.CompilerServices;
using AuditPatternSample.Models;

namespace AuditPatternSample.Services;

public abstract class AuditableService(ILogger logger, AuditQueue auditQueue)
{
    protected void WriteAudit(AuditContext context)
    {
        try
        {
            var entry = new AuditEntry
            {
                Method = context.MethodName,
                Status = context.Status.ToString(),
                Timestamp = context.Timestamp,
                IpAddress = context.IpAddress,
                ErrorMessage = context.Exception?.Message
            };

            auditQueue.Enqueue(entry);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Audit logging failed.");
        }
    }

    protected string GetClientIp(HttpRequest request)
    {
        return request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected string GetCurrentMethod()
    {
        return new StackTrace().GetFrame(1)?.GetMethod()?.Name ?? "unknown";
    }
}
