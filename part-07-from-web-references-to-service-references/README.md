# Part 07: From Web References to Service References

Article: [Modernizing .NET - Part 7: From Web References to Service References](https://medium.com/@michael.kopt/modernizing-net-part-7-from-web-references-to-service-references-540dcc8352be)

This sample demonstrates the migration path from classic `.NET Framework` Web References to a generated `.NET` SOAP client created with `dotnet-svcutil`.

## Sample Focus

The article's pattern is:

- generate the proxy from WSDL with `dotnet-svcutil`
- wrap the generated client in your own service class
- configure bindings, timeouts, headers, and cookies yourself

This sample includes both sides:

- a small CoreWCF host that publishes `SoapService.asmx`
- a console client that consumes the service through a generated `Reference.cs`

## Structure

- `src/SoapServiceHost` hosts a SOAP service and records incoming headers and cookies
- `src/SoapClientSample` contains the generated service reference and a wrapper around it
- `dotnet-tools.json` pins `dotnet-svcutil` locally for this sample

## Implementation Notes

The client wrapper demonstrates:

- `dotnet-svcutil`-generated proxy usage
- custom binding creation
- shared timeout configuration
- custom HTTP headers through `OperationContextScope`
- cookie support through `IHttpCookieContainerManager`

The host exposes debug endpoints so you can verify the wrapper behavior:

- `GET /debug/last-request`
- `POST /debug/reset`

## Run

1. Start the host:

```powershell
dotnet run --project .\src\SoapServiceHost\SoapServiceHost.csproj
```

2. In another terminal, run the client:

```powershell
dotnet run --project .\src\SoapClientSample\SoapClientSample.csproj
```

## Generated Proxy Command

The service reference in this sample was generated with the same shape described in the article:

```powershell
dotnet tool run dotnet-svcutil .\ServiceReference\SoapService.wsdl --sync --wrapped --namespace "*,Part07.SoapClientSample.ServiceReference" --serializer XmlSerializer --outputDir .\ServiceReference
```

## Expected Result

The client should:

- call `Login`
- print `LOGIN_OK:demo-token`
- send a custom HTTP header
- send a cookie

The host debug endpoint should show the received header and cookie values.
