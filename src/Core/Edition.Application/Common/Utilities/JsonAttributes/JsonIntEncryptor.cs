using System.Text.Json;
using System.Text.Json.Serialization;
using Edition.Application.Common.Utilities.Security;

namespace Edition.Application.Common.Utilities.JsonAttributes;

public class JsonIntEncryptor(string key) : JsonConverter<int>
{
    public string Key { get; } = key;

    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var encrypted = reader.GetString();
        var decrypted = encrypted.Decrypt(Key);
        int.TryParse(decrypted, out var integerValue);
        return integerValue;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        var stringValue = value.ToString();
        var encrypted = stringValue.Encrypt(Key);
        writer.WriteStringValue(encrypted);
    }
}
public class JsonNullableIntEncryptor : JsonConverter<int?>
{
    public string Key { get; }

    public JsonNullableIntEncryptor(string key)
    {
        Key = key;
    }
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var encrypted = reader.GetString();
        var decrypted = encrypted.Decrypt(Key);
        if (string.IsNullOrEmpty(encrypted))
            return null;
        int.TryParse(decrypted, out var integerValue);
        return integerValue;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        var stringValue = "";
        if (value != null)
            stringValue = value.ToString();
        var encrypted = stringValue.Encrypt(Key);
        writer.WriteStringValue(encrypted);
    }
}