using ProtoBuf;

namespace ProtobufMigrationSample.Models;

[ProtoContract]
public sealed class UserV1
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = string.Empty;
}
