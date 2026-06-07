# Part 22: Migrating Configuration Files to .NET Core

Article: [Modernizing .NET - Part 22 Migrating Configuration Files to .NET Core](https://medium.com/@michael.kopt/modernizing-net-part-22-migrating-configuration-files-to-net-core-0f7436c938b4)

This sample demonstrates a compatibility-first migration path for legacy configuration files. It keeps one `.properties` file and one XML-based `.config` file, exposes them through `IConfiguration`, and also mirrors values into environment variables so old static access patterns can keep working during the transition.

## Sample Focus

- A custom configuration provider for legacy `key=value` `.properties` files.
- A reusable XML key/value provider for legacy `.config` files.
- A startup pattern that loads `appsettings.json`, then legacy files, then environment variables for final overrides.
- A small HTTP endpoint that shows the effective configuration and the mirrored environment variables.
- Reload-on-change support through `FileConfigurationProvider`.

## Implementation Notes

- `legacy.properties` is parsed into flat keys such as `DB_HOST` and `SMTP_HOST`.
- `legacy.config` shows the XML key/value case through one placeholder section name.
- The same XML key/value provider could load roots such as `mailSettings`, `overrides`, or any other `<add key="..." value="..."/>` shape.
- Each provider also calls `Environment.SetEnvironmentVariable(...)` so legacy code paths can keep using `Environment.GetEnvironmentVariable(...)` during the migration.

## Structure

```text
src/ConfigurationMigrationSample
tests/ConfigurationMigrationSample.Tests
```

## Run

```powershell
dotnet run --project .\src\ConfigurationMigrationSample
```

The sample listens on `http://localhost:5888`.

## Try It

Inspect the effective merged configuration:

```powershell
Invoke-RestMethod http://localhost:5888/config
```

Override one value through an environment variable before startup:

```powershell
$env:SMTP_HOST='smtp.override.local'
dotnet run --project .\src\ConfigurationMigrationSample
```

Trigger reload behavior by editing one of the legacy files and then requesting the endpoint again:

```powershell
notepad .\src\ConfigurationMigrationSample\LegacyConfig\legacy.properties
Invoke-RestMethod http://localhost:5888/config
```

## Notes

- The point of the sample is not to preserve old file formats forever. It shows how to bridge them safely while code and deployment move toward `appsettings.json`, secret stores, and environment variables.
- The providers are intentionally small and explicit so they are easy to adapt to project-specific file shapes.
