# Part 05: Documentation in CoreWCF

Article: [Modernizing .NET - Part 5: Documentation in CoreWCF](https://medium.com/@michael.kopt/modernizing-net-part-5-documentation-in-corewcf-24814b40e37e)

This sample extends the CoreWCF service from Parts 3 and 4 with a browser-facing documentation page that mimics the old `.asmx` experience.

## Key Idea

CoreWCF does not generate the familiar ASP.NET Framework documentation page for SOAP services, so this sample builds one from:

- reflection over the service contract
- `[Description]` attributes on operations
- WSDL fetched from the running service

The generated HTML is cached in memory so documentation requests do not rebuild the page every time.

## Project Layout

- `src/CoreWcfDocsSample/Program.cs` wires CoreWCF, WSDL customization, and the docs endpoint
- `src/CoreWcfDocsSample/Services/SoapDocumentationService.cs` builds and caches HTML documentation
- `src/CoreWcfDocsSample/Middleware/WsdlCustomizerMiddleware.cs` keeps the legacy-friendly WSDL behavior from Part 4
- `src/CoreWcfDocsSample/Contracts/ISoapService.cs` provides the reflected operation descriptions

## What The Sample Does

The service exposes:

- `http://localhost:5380/SoapService.asmx` for the operation list
- `http://localhost:5380/SoapService.asmx?op=Login` for per-operation documentation
- `http://localhost:5380/SoapService.asmx?wsdl` for the rewritten WSDL

The documentation page includes:

- the method name
- the description from `[Description]`
- a sample SOAP request
- a sample SOAP response

## Run

From this folder:

```powershell
dotnet run --project .\src\CoreWcfDocsSample\CoreWcfDocsSample.csproj
```

Then open:

- `http://localhost:5380/SoapService.asmx`
- `http://localhost:5380/SoapService.asmx?op=Login`

## Caveat

This is a compatibility page, not a general documentation framework. It is useful when consumers are used to legacy `.asmx` help pages, but the generated HTML is coupled to your current contract and WSDL shape.
