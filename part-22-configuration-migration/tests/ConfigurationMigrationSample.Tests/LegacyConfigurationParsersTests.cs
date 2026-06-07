using System.Text;
using ConfigurationMigrationSample.Configuration;
using Xunit;

namespace ConfigurationMigrationSample.Tests;

public class LegacyConfigurationParsersTests
{
    [Fact]
    public void ParseProperties_LoadsKeyValuePairsAndSkipsComments()
    {
        Environment.SetEnvironmentVariable("DB_HOST", null);
        Environment.SetEnvironmentVariable("SMTP_HOST", null);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("""
            # comment
            DB_HOST=legacy-db.local
            SMTP_HOST=smtp.legacy.local
            invalid-line
            """));

        var data = LegacyConfigurationParsers.ParseProperties(stream);

        Assert.Equal("legacy-db.local", data["DB_HOST"]);
        Assert.Equal("smtp.legacy.local", data["SMTP_HOST"]);
        Assert.Equal("legacy-db.local", Environment.GetEnvironmentVariable("DB_HOST"));
        Assert.Equal("smtp.legacy.local", Environment.GetEnvironmentVariable("SMTP_HOST"));
    }

    [Fact]
    public void ParseXmlKeyValue_LoadsGenericLegacySection()
    {
        Environment.SetEnvironmentVariable("LOG_LEVEL", null);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("""
            <legacySettings>
              <add key="LOG_LEVEL" value="Debug" />
              <add key="CacheEnabled" value="true" />
            </legacySettings>
            """));

        var data = LegacyConfigurationParsers.ParseXmlKeyValue(stream, "legacySettings", "key", "value");

        Assert.Equal("Debug", data["LOG_LEVEL"]);
        Assert.Equal("true", data["CacheEnabled"]);
        Assert.Equal("Debug", Environment.GetEnvironmentVariable("LOG_LEVEL"));
    }
}
