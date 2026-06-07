# Part 16: Replacing BinaryFormatter with protobuf-net

Article: [Modernizing .NET - Part 16 Replacing BinaryFormatter with protobuf-net](https://medium.com/@michael.kopt)

This sample demonstrates the practical migration path from `BinaryFormatter` to `protobuf-net` in modern .NET. It focuses on the common replacement scenarios from the article: session-style byte storage, file persistence, inheritance, and version-tolerant reads.

## Sample Focus

- A `BinaryUtils` helper that replaces `BinaryFormatter`-style object-to-byte-array and stream conversions.
- `protobuf-net` model annotations with `ProtoContract`, `ProtoMember`, and `ProtoInclude`.
- A session-style wrapper that stores protobuf payloads as `byte[]`.
- File-based serialization and deserialization.
- Backward-compatible deserialization from a V1 payload into a V2 model.

## Implementation Notes

- The sample uses `protobuf-net` package version `3.2.26`, matching the article draft.
- The session example uses a simple in-memory dictionary so the replacement pattern is visible without needing ASP.NET Core session infrastructure.
- The sample does not deserialize legacy `BinaryFormatter` payloads, because the goal here is the modern replacement pattern rather than a risky compatibility bridge.

## Structure

```text
src/ProtobufMigrationSample
```

## Run

Session-style byte storage:

```powershell
dotnet run --project .\src\ProtobufMigrationSample\ProtobufMigrationSample.csproj -- session
```

File persistence:

```powershell
dotnet run --project .\src\ProtobufMigrationSample\ProtobufMigrationSample.csproj -- file .\tests\order.bin
```

Inheritance with `ProtoInclude`:

```powershell
dotnet run --project .\src\ProtobufMigrationSample\ProtobufMigrationSample.csproj -- inheritance
```

Versioning from `UserV1` payload to `UserV2` model:

```powershell
dotnet run --project .\src\ProtobufMigrationSample\ProtobufMigrationSample.csproj -- versioning
```

Run all demos:

```powershell
dotnet run --project .\src\ProtobufMigrationSample\ProtobufMigrationSample.csproj -- all
```

## Notes

- `protobuf-net` requires explicit contracts. Missing annotations are one of the most common migration mistakes.
- Field numbers are the compatibility boundary. New fields are safe; reusing or changing existing numbers is not.
- The current `protobuf-net` 3.x package does not make reference-tracking a good default migration path here, so this sample stays with acyclic DTO-style contracts.
- For real cyclic object graphs, Part 15's in-memory reference store is often the safer modernization bridge than forcing binary serialization.
