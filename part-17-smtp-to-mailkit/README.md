# Part 17: Migrating SMTP Email to MailKit

Article: [Modernizing .NET - Part 17 Migrating SMTP Email to MailKit](https://medium.com/@michael.kopt/modernizing-net-part-17-migrating-smtp-email-to-mailkit-68b17e8a743f)

This sample demonstrates a practical migration from legacy `System.Net.Mail` usage to `MailKit` and `MimeKit`. It keeps a familiar wrapper surface so existing application code can stay close to the old API while the transport is modernized underneath.

## Sample Focus

- A wrapper `SmtpClient` with `Host`, `Port`, `EnableSsl`, and `Credentials`, similar to the old `System.Net.Mail.SmtpClient`.
- Wrapper `MailMessage` and `MailAddress` types that are mapped to `MimeMessage`.
- Async-first sending through MailKit.
- Environment-variable configuration for SMTP settings.
- A preview mode that writes an `.eml` file locally, so the migration can be exercised without a live SMTP server.

## Implementation Notes

- The article examples use `4.3.0`, but this sample uses current non-vulnerable packages from NuGet so the repo does not pin known-bad versions.
- `preview` mode validates message mapping and MIME generation without opening a network connection.
- `send` mode uses real MailKit SMTP operations and only works when SMTP environment variables are configured.
- The wrapper intentionally covers the common migration path, not every edge of the original `System.Net.Mail` API.

## Structure

```text
src/MailKitMigrationSample
```

## Run

Generate a local `.eml` preview:

```powershell
dotnet run --project .\src\MailKitMigrationSample\MailKitMigrationSample.csproj -- preview
```

Show resolved SMTP configuration:

```powershell
dotnet run --project .\src\MailKitMigrationSample\MailKitMigrationSample.csproj -- config
```

Send using a real SMTP server:

```powershell
$env:SMTP_HOST = "smtp.example.com"
$env:SMTP_PORT = "587"
$env:SMTP_SSL = "true"
$env:SMTP_USER = "user"
$env:SMTP_PASS = "pass"
dotnet run --project .\src\MailKitMigrationSample\MailKitMigrationSample.csproj -- send
```

## Notes

- Port `587` with STARTTLS is the normal modern default.
- Many providers require app passwords or provider-specific SMTP credentials.
- Preview mode is included because a migration sample should be runnable even without external infrastructure.
