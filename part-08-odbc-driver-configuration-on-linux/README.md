# Part 08: ODBC Driver Configuration on Linux

Article: [Modernizing .NET - Part 8: ODBC Driver Configuration on Linux](https://medium.com/@michael.kopt/%EF%B8%8F-modernizing-net-part-8-odbc-driver-configuration-on-linux-a9083eabc5bd)

This sample is configuration-focused rather than application-focused. It shows how to structure Linux ODBC setup for multiple vendors and includes a tiny `.NET` smoke test that verifies whether `System.Data.Odbc` can open a connection.

## Structure

- `config/odbcinst.ini` contains example unixODBC driver registrations
- `config/odbc.ini` contains example DSNs
- `config/env.sh` and `config/env.ps1` set the environment variables unixODBC relies on
- `scripts/` contains install snippets for each driver from the article
- `docker/` contains example Dockerfile fragments for each driver
- `src/OdbcSmokeTest` is a minimal console app that attempts to open an ODBC connection

## Implementation Notes

unixODBC driver definitions:

- `config/odbcinst.ini`

DSN definitions:

- `config/odbc.ini`

Environment variables:

- `ODBCINI`
- `ODBCSYSINI`
- `AMAZONREDSHIFTODBCINI`

## Run

Build and run:

```powershell
dotnet run --project .\src\OdbcSmokeTest\OdbcSmokeTest.csproj -- "Driver={ODBC Driver 18 for SQL Server};Server=localhost;Database=master;Uid=sa;Pwd=your-password;TrustServerCertificate=Yes;"
```

The app prints:

- whether the connection opened successfully
- provider-level error details if it failed

This is useful for validating that:

- unixODBC is installed
- the driver name matches your config
- the native `.so` library can actually be loaded

## Notes

- These examples are intentionally vendor-specific and Linux-oriented.
- Some drivers, like Amazon Redshift ODBC, are `x86_64` only.
- The install snippets use placeholder URLs where the article omitted the full download link.
- The smoke test compiles anywhere, but opening a connection requires the native driver and target server to be available.
