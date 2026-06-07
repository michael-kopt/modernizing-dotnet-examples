# Part 04: WSDL in CoreWCF

Article: [Modernizing .NET - Part 4: WSDL in CoreWCF](https://medium.com/@michael.kopt/modernizing-net-part-4-wsdl-in-corewcf-3b5e3a390c37)

This sample builds on the CoreWCF SOAP service from Part 3 and adds a middleware layer that rewrites the generated WSDL into a shape that is closer to what legacy SOAP clients expect.

## Sample Focus

CoreWCF can host the SOAP service, but the generated WSDL often differs from the old `.NET Framework` output in ways that matter to existing clients.

This sample demonstrates the same categories of fixes described in the article:

- redirect `?wsdl` requests to `?singleWsdl`
- simplify verbose message names
- rewrite the published endpoint URL
- rename binding and port references
- remove WS-Policy nodes that older clients may mis-handle

## Structure

- `src/CoreWcfWsdlSample/Program.cs` wires CoreWCF and the WSDL middleware
- `src/CoreWcfWsdlSample/Middleware/WsdlCustomizerMiddleware.cs` captures and rewrites the XML
- `src/CoreWcfWsdlSample/Contracts/ISoapService.cs` defines the SOAP contract
- `src/CoreWcfWsdlSample/Services/SoapServiceController.cs` implements the SOAP operation

## Implementation Notes

The service exposes:

- `http://localhost:5280/SoapService.asmx`
- `http://localhost:5280/SoapService.asmx?wsdl`

The WSDL middleware rewrites the generated output so that:

- `SoapService_Login_InputMessage` becomes `LoginSoapIn`
- `SoapService_Login_OutputMessage` becomes `LoginSoapOut`
- `BasicHttpBinding_SoapService` becomes `SoapServiceSoap`
- the published SOAP address uses `baseURL` when provided
- policy nodes are removed

## Run

From this folder:

```powershell
dotnet run --project .\src\CoreWcfWsdlSample\CoreWcfWsdlSample.csproj
```

If you want the WSDL to publish a reverse-proxy URL, set:

```powershell
$env:baseURL = 'https://api.mycompany.com/'
dotnet run --project .\src\CoreWcfWsdlSample\CoreWcfWsdlSample.csproj
```

Then open:

- `http://localhost:5280/SoapService.asmx?wsdl`

## Notes

This kind of middleware is a compatibility bridge. It is useful when external consumers are sensitive to WSDL shape, but it also means you now own a custom transformation layer that should be tested whenever the service contract changes.
