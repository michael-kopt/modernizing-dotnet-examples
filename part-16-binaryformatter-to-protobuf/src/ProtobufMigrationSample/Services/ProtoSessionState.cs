namespace ProtobufMigrationSample.Services;

public sealed class ProtoSessionState
{
    private readonly Dictionary<string, byte[]> values = new(StringComparer.OrdinalIgnoreCase);

    public void Set<T>(string key, T value)
    {
        var bytes = BinaryUtils.ObjectToByteArray(value)
            ?? throw new InvalidOperationException("Cannot store a null value in ProtoSessionState.");

        values[key] = bytes;
    }

    public T? Get<T>(string key)
    {
        return !values.TryGetValue(key, out var bytes)
            ? default
            : BinaryUtils.ByteArrayToObject<T>(bytes);
    }

    public byte[]? GetRaw(string key)
    {
        return values.TryGetValue(key, out var bytes) ? bytes : null;
    }
}
