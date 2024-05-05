using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tapo.Application.Protocol;

public class TapoJsonDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s != null)
            return DateTime.Parse(s);
        else
            throw new JsonException($"Reader returned null for DateTime string {s}.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
    }
}
