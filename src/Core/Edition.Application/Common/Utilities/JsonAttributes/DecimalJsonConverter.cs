using Newtonsoft.Json;

namespace Edition.Application.Common.Utilities.JsonAttributes;

public class DecimalJsonConverter : JsonConverter
{
    public override bool CanRead => false;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(decimal) || objectType == typeof(float) || objectType == typeof(double));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (IsWholeValue(value))
        {
            writer.WriteRawValue(JsonConvert.ToString(Convert.ToInt64(value)));
        }
        else
        {
            writer.WriteRawValue(JsonConvert.ToString(value));
        }
    }

    private static bool IsWholeValue(object value)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue == Math.Truncate(decimalValue);
        }
        else if (value is float floatValue)
        {
            return floatValue == Math.Truncate(floatValue);
        }
        else if (value is double doubleValue)
        {
            return doubleValue == Math.Truncate(doubleValue);
        }

        return false;
    }
}