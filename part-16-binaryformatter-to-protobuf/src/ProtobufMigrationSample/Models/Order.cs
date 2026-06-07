using ProtoBuf;

namespace ProtobufMigrationSample.Models;

[ProtoContract]
public sealed class Order
{
    [ProtoMember(1)]
    public int OrderId { get; set; }

    [ProtoMember(2)]
    public List<OrderItem> Items { get; set; } = [];

    [ProtoMember(3)]
    public Dictionary<string, string> Metadata { get; set; } = [];
}
