using ProtoBuf;

namespace ProtobufMigrationSample.Models;

[ProtoContract]
public sealed class Developer : Employee
{
    [ProtoMember(3)]
    public string ProgrammingLanguage { get; set; } = string.Empty;
}
