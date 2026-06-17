namespace SystemWebWrappersSample.Legacy;

public static class LegacyResponseWriter
{
    public static async Task<LegacyResponseSnapshot> WriteExportAsync()
    {
        var request = new System.Web.HttpRequest();
        var response = new System.Web.HttpResponse();

        var format = request.Params["format"] ?? "json";

        response.ContentType = string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase)
            ? "text/csv"
            : "application/json";

        response.AppendHeader("X-Legacy-Format", format);
        response.AddHeader("X-Legacy-Wrapper", "System.Web.HttpResponse");

        var payload = string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase)
            ? "id,name\r\n42,legacy-export"
            : "{\"id\":42,\"name\":\"legacy-export\"}";

        await response.WriteAsync(payload);

        return new LegacyResponseSnapshot(
            ContentType: response.ContentType ?? string.Empty,
            IsClientConnected: response.IsClientConnected,
            Headers: response.Headers.ToDictionary(header => header.Key, header => header.Value.ToString(), StringComparer.OrdinalIgnoreCase),
            BodyPreview: payload);
    }
}

public sealed record LegacyResponseSnapshot(
    string ContentType,
    bool IsClientConnected,
    IReadOnlyDictionary<string, string> Headers,
    string BodyPreview);
