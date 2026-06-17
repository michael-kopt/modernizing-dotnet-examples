namespace SystemWebWrappersSample.Legacy;

public static class LegacyRequestReader
{
    public static async Task<LegacyRequestSnapshot> ReadAsync()
    {
        var request = new System.Web.HttpRequest();

        using var reader = new StreamReader(request.InputStream, leaveOpen: true);
        request.InputStream.Position = 0;
        var body = await reader.ReadToEndAsync();
        request.InputStream.Position = 0;

        return new LegacyRequestSnapshot(
            Method: request.HttpMethod,
            RawUrl: request.RawUrl,
            Path: request.Path,
            UserHostAddress: request.UserHostAddress,
            UserHostName: request.UserHostName,
            IsSecureConnection: request.IsSecureConnection,
            Params: request.Params.AllKeys
                .Where(key => key is not null)
                .ToDictionary(key => key!, key => request.Params[key!] ?? string.Empty, StringComparer.OrdinalIgnoreCase),
            Body: body);
    }
}

public sealed record LegacyRequestSnapshot(
    string Method,
    string RawUrl,
    string Path,
    string? UserHostAddress,
    string? UserHostName,
    bool IsSecureConnection,
    IReadOnlyDictionary<string, string> Params,
    string Body);
