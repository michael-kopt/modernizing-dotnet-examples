using ProtoBuf;

namespace ProtobufMigrationSample.Services;

public static class BinaryUtils
{
    public static byte[]? ObjectToByteArray<T>(T obj)
    {
        if (obj is null)
        {
            return null;
        }

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, obj);
        return stream.ToArray();
    }

    public static Stream? ObjectToStream<T>(T obj)
    {
        if (obj is null)
        {
            return null;
        }

        var stream = new MemoryStream();
        Serializer.Serialize(stream, obj);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public static T StreamToObject<T>(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        return Serializer.Deserialize<T>(stream);
    }

    public static T ByteArrayToObject<T>(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        return Serializer.Deserialize<T>(stream);
    }
}
