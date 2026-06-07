namespace Part07.SoapServiceHost.Services;

public sealed class RequestRecorder
{
    private string? _lastClientName;
    private string? _lastSessionId;

    public void Record(string? clientName, string? sessionId)
    {
        _lastClientName = clientName;
        _lastSessionId = sessionId;
    }

    public object GetSnapshot()
    {
        return new
        {
            clientName = _lastClientName,
            sessionId = _lastSessionId
        };
    }

    public void Reset()
    {
        _lastClientName = null;
        _lastSessionId = null;
    }
}
