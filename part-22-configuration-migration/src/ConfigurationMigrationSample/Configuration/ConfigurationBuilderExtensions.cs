using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace ConfigurationMigrationSample.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddLegacyPropertiesFile(
        this IConfigurationBuilder builder,
        string path,
        bool optional,
        bool reloadOnChange)
    {
        return builder.Add(new LegacyPropertiesConfigurationSource
        {
            FileProvider = CreateFileProvider(path),
            Path = Path.GetFileName(path),
            Optional = optional,
            ReloadOnChange = reloadOnChange
        });
    }

    public static IConfigurationBuilder AddLegacyXmlKeyValueFile(
        this IConfigurationBuilder builder,
        string path,
        string parentElementName,
        string keyAttributeName,
        string valueAttributeName,
        bool optional,
        bool reloadOnChange)
    {
        return builder.Add(new LegacyXmlKeyValueConfigurationSource
        {
            FileProvider = CreateFileProvider(path),
            Path = Path.GetFileName(path),
            Optional = optional,
            ReloadOnChange = reloadOnChange,
            ParentElementName = parentElementName,
            KeyAttributeName = keyAttributeName,
            ValueAttributeName = valueAttributeName
        });
    }

    private static PhysicalFileProvider CreateFileProvider(string path)
    {
        var directory = Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException($"Cannot resolve directory for path '{path}'.");

        return new PhysicalFileProvider(directory);
    }
}
