# Part 03: Surviving SOAP with CoreWCF

Article: [Modernizing .NET - Part 3: Surviving SOAP with CoreWCF](https://medium.com/@michael.kopt/modernizing-net-part-3-surviving-soap-with-corewcf-8a947ebd55e9)

This sample shows the basic migration path from a legacy `.asmx` or WCF-style SOAP endpoint to a CoreWCF service hosted in `.NET 8`.

## Key Idea

The article describes three essential steps:

- define a SOAP contract that matches the legacy service
- implement that contract in a service class
- expose it through CoreWCF using `BasicHttpBinding`

The contract in this sample follows the same pattern as the article, including:

- `ServiceContract`
- `OperationContract`
- `XmlSerializerFormat`

That combination is the important part when you want CoreWCF to behave more like classic .NET Framework SOAP services.

## Project Layout

- `src/CoreWcfSoapSample/Contracts/ISoapService.cs` defines the SOAP contract
- `src/CoreWcfSoapSample/Services/SoapServiceController.cs` implements the service
- `src/CoreWcfSoapSample/Program.cs` wires CoreWCF into the ASP.NET Core pipeline

## What The Sample Does

The service exposes:

- `SoapService.asmx`
- `Login(username, password)`

For demonstration purposes, `Login` uses a simple in-memory credential check:

- `demo` / `pass123` returns a success token
- any other combination returns `INVALID_CREDENTIALS`

## Run

From this folder:

```powershell
dotnet run --project .\src\CoreWcfSoapSample\CoreWcfSoapSample.csproj
```

Service endpoint:

- `http://localhost:5180/SoapService.asmx`

## Example SOAP Request

```xml
POST /SoapService.asmx HTTP/1.1
Host: localhost:5180
Content-Type: text/xml; charset=utf-8
SOAPAction: "http://www.namespace.com/Login"

<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
               xmlns:xsd="http://www.w3.org/2001/XMLSchema"
               xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <Login xmlns="http://www.namespace.com/">
      <username>demo</username>
      <password>pass123</password>
    </Login>
  </soap:Body>
</soap:Envelope>
```

## Caveat

This is the functional starting point, not full .NET Framework parity. As the article notes, WSDL shape, metadata pages, and validation behavior still need extra work.
