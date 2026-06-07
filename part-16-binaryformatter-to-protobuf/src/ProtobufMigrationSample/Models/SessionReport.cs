using ProtoBuf;

namespace ProtobufMigrationSample.Models;

[ProtoContract]
public sealed class SessionReport
{
    [ProtoMember(1)]
    public string ReportName { get; set; } = string.Empty;

    [ProtoMember(2)]
    public DateTime CreatedAtUtc { get; set; }

    [ProtoMember(3)]
    public List<string> Sections { get; set; } = [];
}
