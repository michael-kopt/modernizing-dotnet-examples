using ProtoBuf;

namespace ProtobufMigrationSample.Models;

[ProtoContract]
[ProtoInclude(100, typeof(Manager))]
[ProtoInclude(101, typeof(Developer))]
public class Employee
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = string.Empty;
}
