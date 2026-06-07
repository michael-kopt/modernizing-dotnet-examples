# Part 06: Validators in CoreWCF

Article: [Modernizing .NET - Part 6: Validators in CoreWCF](https://medium.com/@michael.kopt/modernizing-net-part-6-validators-in-corewcf-50c0eb2f2feb)

This sample recreates the article's attribute-based SOAP validation pattern for CoreWCF by attaching a custom `IParameterInspector` before the service method executes.

## Key Idea

The article breaks the solution into four pieces:

- an `AssertAttribute` that declares validation rules on a SOAP method
- an `AssertValidator` that evaluates one rule
- a CoreWCF `IParameterInspector` that runs before method execution
- a behavior attribute that plugs the inspector into the dispatch pipeline

This sample keeps that structure and demonstrates that invalid SOAP requests are rejected before business logic runs.

## Project Layout

- `src/CoreWcfValidationSample/Validation/AssertAttribute.cs` defines the assertion model
- `src/CoreWcfValidationSample/Validation/AssertValidator.cs` executes comparisons
- `src/CoreWcfValidationSample/Validation/CoreWcfParameterInspector.cs` validates incoming parameters
- `src/CoreWcfValidationSample/Validation/ValidationAttribute.cs` attaches the inspector to the operation
- `src/CoreWcfValidationSample/Contracts/ISoapService.cs` applies the assertions to `Login`

## What The Sample Does

The `Login` operation has declarative rules:

- `username` must not be empty
- `username` must be shorter than `100`
- `password` must be longer than `3`
- `password` must be shorter than `20`

If validation fails, the service returns a SOAP fault and the legacy login handler is not called.

For verification, the app also exposes:

- `GET /debug/call-count`
- `POST /debug/reset`

These endpoints let you confirm whether the SOAP method actually executed.

## Run

From this folder:

```powershell
dotnet run --project .\src\CoreWcfValidationSample\CoreWcfValidationSample.csproj
```

SOAP endpoint:

- `http://localhost:5480/SoapService.asmx`

Debug endpoints:

- `http://localhost:5480/debug/call-count`

## Expected Behavior

- valid SOAP requests return `LOGIN_OK:demo-token`
- invalid SOAP requests return a SOAP fault
- invalid requests do not increase the debug call count

## Note

The article mentions pre-deserialization validation from legacy `SoapExtension` pipelines. CoreWCF does not expose the same raw-XML hook in this sample, so the validation runs before the service method executes but after CoreWCF has mapped inputs to method arguments.
