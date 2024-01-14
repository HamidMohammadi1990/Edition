using System.Text.Json;
using System.Text.Json.Serialization;
using Edition.Application.Common.Utilities.Security;

namespace Edition.Application.Common.Utilities.JsonAttributes;

public class JsonStringEncryptor(string key) : JsonConverter<string>
{
    public string Key { get; } = key;

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var encrypted = reader.GetString();
        var decrypted = encrypted!.Decrypt(Key);
        return decrypted;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        var encrypted = value.Encrypt(Key);
        writer.WriteStringValue(encrypted);
    }
}