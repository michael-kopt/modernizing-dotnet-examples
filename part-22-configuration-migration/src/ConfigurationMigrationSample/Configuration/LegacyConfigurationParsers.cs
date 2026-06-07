using System.Xml.Linq;

namespace ConfigurationMigrationSample.Configuration;

public static class LegacyConfigurationParsers
{
    public static IDictionary<string, string?> ParseProperties(Stream stream)
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var rawLine = reader.ReadLine();
            var line = rawLine?.Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            data[key] = value;
            Environment.SetEnvironmentVariable(key, value);
        }

        return data;
    }

    public static IDictionary<string, string?> ParseXmlKeyValue(
        Stream stream,
        string parentElementName,
        string keyAttributeName,
        string valueAttributeName)
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var document = XDocument.Load(stream);
        var parent = document.Element(parentElementName);
        var entries = parent?.Elements("add") ?? Enumerable.Empty<XElement>();

        foreach (var entry in entries)
        {
            var key = entry.Attribute(keyAttributeName)?.Value;
            var value = entry.Attribute(valueAttributeName)?.Value;

            if (string.IsNullOrWhiteSpace(key) || value is null)
            {
                continue;
            }

            data[key] = value;
            Environment.SetEnvironmentVariable(key, value);
        }

        return data;
    }
}
