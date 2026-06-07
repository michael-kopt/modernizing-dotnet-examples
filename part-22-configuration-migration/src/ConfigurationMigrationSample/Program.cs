using ConfigurationMigrationSample.Configuration;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var legacyConfigPath = Path.Combine(builder.Environment.ContentRootPath, "LegacyConfig");

if (Directory.Exists(legacyConfigPath))
{
    builder.Configuration
        .AddLegacyPropertiesFile(Path.Combine(legacyConfigPath, "legacy.properties"), optional: true, reloadOnChange: true)
        .AddLegacyXmlKeyValueFile(Path.Combine(legacyConfigPath, "legacy.config"), "legacySettings", "key", "value", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/config"));

app.MapGet("/config", (IConfiguration configuration) =>
{
    var payload = new
    {
        SourceOrder = new[]
        {
            "appsettings.json",
            "LegacyConfig/legacy.properties",
            "LegacyConfig/legacy.config",
            "environment variables"
        },
        EffectiveConfiguration = new
        {
            DbHost = configuration["DB_HOST"],
            DbPort = configuration["DB_PORT"],
            SmtpHost = configuration["SMTP_HOST"],
            FeatureNewUi = configuration["FEATURE_NEW_UI"],
            LogLevel = configuration["LOG_LEVEL"],
            SupportInbox = configuration["SupportInbox"]
        },
        LegacyEnvironmentVariables = new
        {
            DbHost = Environment.GetEnvironmentVariable("DB_HOST"),
            SmtpHost = Environment.GetEnvironmentVariable("SMTP_HOST"),
            SupportInbox = Environment.GetEnvironmentVariable("SupportInbox")
        }
    };

    return Results.Json(payload);
});

app.Run("http://localhost:5888");
