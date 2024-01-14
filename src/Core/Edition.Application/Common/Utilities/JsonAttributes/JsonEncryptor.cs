using System.Text.Json;
using System.Text.Json.Serialization;
using Edition.Application.Common.Utilities.Security;

namespace Edition.Application.Common.Utilities.JsonAttributes;

public class JsonEncryptor : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var encrypted = reader.GetString();
        var decrypted = encrypted.Decrypt();
        _ = Guid.TryParse(decrypted, out var value);
        return value;
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        var encrypted = value.ToString().Encrypt();
        writer.WriteStringValue(encrypted);
    }
}