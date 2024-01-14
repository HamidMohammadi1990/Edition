using ProtoBuf;
using StackExchange.Redis;

namespace Edition.Common.Extensions;

public static class SerializerExtension
{
    public static byte[] Serialize<T>(this T item) where T : class
    {
        ValidateModel<T>();
        var memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, item);
        return memoryStream.ToArray();
    }

    public static T? Deserialize<T>(this byte[]? data)
    {
        if (data is null) return default;
        ValidateModel<T>();
        var memoryStream = new MemoryStream(data);
        return Serializer.Deserialize<T>(memoryStream);
    }
    public static T? Deserialize<T>(this RedisValue data)
    {
        if (data == RedisValue.Null) return default;
        ValidateModel<T>();
        var memoryStream = new MemoryStream(data);
        return Serializer.Deserialize<T>(memoryStream);
    }

    private static void ValidateModel<T>()
    {
#if DEBUG
        var hasAttribute = AttributeExtension.HasAttribute<T, ProtoContractAttribute>();
        //if (!hasAttribute) throw new Exception("ProtoContract attribute is eligible to cache-able models");
#endif
    }
}