using Microsoft.Extensions.Configuration;

namespace ConfigurationMigrationSample.Configuration;

public sealed class LegacyXmlKeyValueConfigurationSource : FileConfigurationSource
{
    public required string ParentElementName { get; init; }

    public required string KeyAttributeName { get; init; }

    public required string ValueAttributeName { get; init; }

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new LegacyXmlKeyValueConfigurationProvider(this);
    }
}

public sealed class LegacyXmlKeyValueConfigurationProvider : FileConfigurationProvider
{
    private readonly LegacyXmlKeyValueConfigurationSource _source;

    public LegacyXmlKeyValueConfigurationProvider(LegacyXmlKeyValueConfigurationSource source)
        : base(source)
    {
        _source = source;
    }

    public override void Load(Stream stream)
    {
        Data = LegacyConfigurationParsers.ParseXmlKeyValue(
            stream,
            _source.ParentElementName,
            _source.KeyAttributeName,
            _source.ValueAttributeName);
    }
}
