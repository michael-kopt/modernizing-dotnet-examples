# Part 01: C# Preprocessor Directives

Article: [Modernizing .NET - Part 1: C# Preprocessor Directives](https://medium.com/@michael.kopt/modernizing-net-part-1-c-preprocessor-directives-0db27fa32d8a)

This sample demonstrates the migration pattern described in the article: one shared codebase compiled by both a `.NET Framework 4.8` project and a `.NET 8` project using explicit custom compilation symbols.

## Sample Focus

Instead of relying on predefined symbols like `NETFRAMEWORK` or `NETCOREAPP`, the projects define their own constants:

```xml
<DefineConstants>$(DefineConstants);PROJECT_DOTNET_CORE</DefineConstants>
```

```csharp
#if PROJECT_DOTNET_CORE
// Code compiled only for .NET 8
#endif
```

```csharp
#if !PROJECT_DOTNET_CORE
// Code compiled only for .NET Framework
#endif
```

```xml
<DefineConstants>DEBUG;TRACE;PROJECT_DOTNET_FRAMEWORK</DefineConstants>
```

```csharp
#if PROJECT_DOTNET_FRAMEWORK
// Code compiled only for .NET Framework
#endif
```

The shared source file uses three simple sections:

- code compiled for both platforms
- code compiled only for `.NET 8`
- code compiled only for `.NET Framework`

## Structure

The article snippets are not runnable on their own, so this folder contains a minimal reproducible example:

- `src/ModernApp` is a `.NET 8` console app
- `src/LegacyApp` is a `.NET Framework 4.8` console app
- `src/Shared/PlatformMessage.cs` is compiled into both projects

It also intentionally avoids `#else` and `#elif`, matching the article guidance.

## Run

From this folder:

```powershell
dotnet run --project .\src\ModernApp\ModernApp.csproj
dotnet run --project .\src\LegacyApp\LegacyApp.csproj
```

If the local machine does not have .NET Framework 4.8 reference assemblies installed, the `LegacyApp` build may require the developer pack.

## Notes

- The sample intentionally uses explicit custom symbols instead of built-in framework symbols.
- Building `LegacyApp` may require the .NET Framework 4.8 developer pack on the local machine.
