using ProtoBuf;

namespace ProtobufMigrationSample.Models;

[ProtoContract]
public sealed class OrderItem
{
    [ProtoMember(1)]
    public string ProductId { get; set; } = string.Empty;

    [ProtoMember(2)]
    public int Quantity { get; set; }

    [ProtoMember(3)]
    public decimal Price { get; set; }
}
