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
            if (reader is { TokenType: JsonToken.String, Value: string str })
            {
                if (TimeOnly.TryParse(str, out var time))
                    return time;
            }
            if (reader is { TokenType: JsonToken.Date, Value: DateTime dt })
            {
                return TimeOnly.FromDateTime(dt);
            }
            throw new JsonSerializationException($"Cannot convert {reader.Value} to TimeOnly");
        }
    }
}
