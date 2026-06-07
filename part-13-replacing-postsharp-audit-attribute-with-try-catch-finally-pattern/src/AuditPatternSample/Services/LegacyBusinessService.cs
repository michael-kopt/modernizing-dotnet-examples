using AuditPatternSample.Models;

namespace AuditPatternSample.Services;

public sealed class LegacyBusinessService(
    ILogger<LegacyBusinessService> logger,
    AuditQueue auditQueue,
    BusinessOperationStore store) : AuditableService(logger, auditQueue)
{
    public string ProcessData(HttpRequest request, string input)
    {
        var audit = new AuditContext(GetCurrentMethod())
        {
            IpAddress = GetClientIp(request)
        };

        try
        {
            if (string.Equals(input, "explode", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Input requested a failure.");
            }

            var result = $"processed:{input}";
            store.Add(result);
            return result;
        }
        catch (Exception ex)
        {
            audit.Status = AuditStatus.Failure;
            audit.Exception = ex;
            throw;
        }
        finally
        {
            WriteAudit(audit);
        }
    }

    public bool CreateItem(HttpRequest request, string name, bool authorized)
    {
        var audit = new AuditContext(GetCurrentMethod())
        {
            IpAddress = GetClientIp(request)
        };

        try
        {
            if (!authorized)
            {
                audit.Status = AuditStatus.Failure;
                audit.Exception = new UnauthorizedAccessException("Caller is not authorized to create items.");
                return false;
            }

            store.Add($"created:{name}");
            return true;
        }
        catch (Exception ex)
        {
            audit.Status = AuditStatus.Failure;
            audit.Exception = ex;
            throw;
        }
        finally
        {
            WriteAudit(audit);
        }
    }

    public void RunInBackground(HttpRequest request, string input)
    {
        var ipAddress = GetClientIp(request);

        Exception? capturedException = null;

        var thread = new Thread(() =>
        {
            var audit = new AuditContext(nameof(RunInBackground))
            {
                IpAddress = ipAddress
            };

            try
            {
                if (string.Equals(input, "explode", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Background work failed.");
                }

                store.Add($"background:{input}");
            }
            catch (Exception ex)
            {
                audit.Status = AuditStatus.Failure;
                audit.Exception = ex;
                capturedException = ex;
            }
            finally
            {
                WriteAudit(audit);
            }
        });

        thread.Start();
        thread.Join();

        if (capturedException is not null)
        {
            throw capturedException;
        }
    }
}
