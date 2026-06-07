# Modernizing .NET Examples

Code examples for the "Modernizing .NET" article series.

![Modernizing .NET series cover](assets/series-cover-2026.png)

## From Legacy to Linux

This repository accompanies a series about migrating a large ASP.NET Framework 4.8 application to .NET 8 on Linux.

Published on Medium:

- [@michael.kopt](https://medium.com/@michael.kopt)

The original system included:

- 1,000+ C# files
- 400,000+ lines of code
- REST and SOAP services
- SAML authentication
- ASPX UI pages
- Multiple ODBC drivers
- Web References
- PostSharp-based AOP
- Google OAuth integration

The goal of the migration was to move a legacy Windows Server and IIS-hosted application to Linux, Kestrel, and a container-friendly deployment model.

This repository focuses on runnable examples and reduced reproductions of the techniques described in the articles.

## Repository Structure

Each article has its own folder with the same part number as the post:

- `part-01-c-preprocessor-directives`
- `part-02-httpcontext-and-the-dark-magic-of-migration`
- `part-03-surviving-soap-with-corewcf`
- `part-04-wsdl-in-corewcf`
- `part-05-documentation-in-corewcf`
- `part-06-validators-in-corewcf`
- `part-07-from-web-references-to-service-references`
- `part-08-odbc-driver-configuration-on-linux`
- `part-09-migrating-saml-sso-to-itfoxtec`
- `part-10-replacing-oledb-excel-reading-with-exceldatareader`
- `part-11-migrating-webclient-and-restsharp-to-httpclientfactory`
- `part-12-managing-dependency-injection-in-background-threads-with-sharedcontext`
- `part-13-replacing-postsharp-audit-attribute-with-try-catch-finally-pattern`
- `part-14-migrating-from-webhost-to-webapplication-in-asp-net-core`
- `part-15-custom-session-store-for-complex-objects-in-asp-net-core`
- `part-16-binaryformatter-to-protobuf`
- `part-17-smtp-to-mailkit`
- `part-18-oauth-authentication`
- `part-19-dictionary-to-redis`
- `part-20-aspx-to-aspnetcore`
- `part-21-webdav-migration`
- `part-22-configuration-migration`
- `part-23-log4net-migration`
- `part-24-prometheus-grafana`
- `part-25-rate-limiting-concepts`
- `part-26-rate-limiting-middleware`
- `part-27-string-sorting`
- `part-28-linux-file-memory-spikes`

Recommended convention inside each part:

- `src/` for runnable sample code
- `tests/` for verification or regression coverage

## Notes

These examples are intentionally separated from the original commercial application. They are meant to illustrate migration patterns, not reproduce the production system.
