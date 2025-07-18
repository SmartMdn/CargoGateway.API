using System;
using Newtonsoft.Json;

namespace CargoGateway.Core.Models.Converters
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string DefaultFormat = "HH:mm:ss";

        public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(DefaultFormat));
        }

        public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String && reader.Value is string str)
            {
                if (TimeOnly.TryParse(str, out var time))
                    return time;
            }
            if (reader.TokenType == JsonToken.Date && reader.Value is DateTime dt)
            {
                return TimeOnly.FromDateTime(dt);
            }
            throw new JsonSerializationException($"Cannot convert {reader.Value} to TimeOnly");
        }
    }
}
