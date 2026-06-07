using Microsoft.Extensions.Configuration;

namespace ConfigurationMigrationSample.Configuration;

public sealed class LegacyPropertiesConfigurationSource : FileConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new LegacyPropertiesConfigurationProvider(this);
    }
}

public sealed class LegacyPropertiesConfigurationProvider : FileConfigurationProvider
{
    public LegacyPropertiesConfigurationProvider(LegacyPropertiesConfigurationSource source)
        : base(source)
    {
    }

    public override void Load(Stream stream)
    {
        Data = LegacyConfigurationParsers.ParseProperties(stream);
    }
}
