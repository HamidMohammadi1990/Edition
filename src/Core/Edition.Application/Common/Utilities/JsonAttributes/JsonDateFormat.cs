using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edition.Application.Common.Utilities.JsonAttributes;

public class JsonDateFormat : IsoDateTimeConverter
{
    public JsonDateFormat(string format) => DateTimeFormat = format;
}
public class JsonDateTimeFormat : IsoDateTimeConverter
{
    public JsonDateTimeFormat(string format) => DateTimeFormat = format;
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var date = DateTime.Parse(reader.Value.ToString()[..10]);
        var timeArray = reader.Value.ToString()[11..].Split(':');
        date = date.AddHours(int.Parse(timeArray[0]));
        date = date.AddMinutes(int.Parse(timeArray[1]));
        return date;
    }
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        base.WriteJson(writer, value, serializer);
    }
}