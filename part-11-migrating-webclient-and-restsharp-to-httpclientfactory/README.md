# Part 11: Migrating WebClient and RestSharp to HttpClientFactory

Article: [Modernizing .NET - Part 11 Migrating WebClient and RestSharp to HttpClientFactory](https://medium.com/@michael.kopt/modernizing-net-part-11-migrating-webclient-and-restsharp-to-httpclientfactory-c3a74091ef19)

## Sample Focus

This sample demonstrates two migration targets from the article:

- a `WebClient`-style abstraction implemented with `IHttpClientFactory`
- a RestSharp-style `ApiClient` implemented with named `HttpClient` instances

It includes a tiny local API so the client sample can verify:

- text download
- byte download
- JSON upload
- custom headers
- cookie behavior
- redirect handling

## Structure

- `src/HttpClientFactoryApi` is a small ASP.NET Core API used as the target server
- `src/HttpClientFactoryClientSample` contains the migrated clients and a console runner

## Run

1. Start the API:

```powershell
dotnet run --project .\src\HttpClientFactoryApi\HttpClientFactoryApi.csproj
```

2. In another terminal, run the client sample:

```powershell
dotnet run --project .\src\HttpClientFactoryClientSample\HttpClientFactoryClientSample.csproj
```

The client prints the results of both migration paths so you can compare behavior.
