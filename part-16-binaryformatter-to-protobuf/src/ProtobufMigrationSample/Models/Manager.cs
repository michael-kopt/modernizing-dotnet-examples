using ProtoBuf;

namespace ProtobufMigrationSample.Models;

[ProtoContract]
public sealed class Manager : Employee
{
    [ProtoMember(3)]
    public int TeamSize { get; set; }
}
