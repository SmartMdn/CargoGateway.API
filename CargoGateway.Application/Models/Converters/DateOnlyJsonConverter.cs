using System;
using Newtonsoft.Json;

namespace CargoGateway.Core.Models.Converters
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DefaultFormat = "yyyy-MM-dd";

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(DefaultFormat));
        }

        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader is { TokenType: JsonToken.String, Value: string str })
            {
                if (DateOnly.TryParse(str, out var date))
                    return date;
            }
            if (reader is { TokenType: JsonToken.Date, Value: DateTime dt })
            {
                return DateOnly.FromDateTime(dt);
            }
            throw new JsonSerializationException($"Cannot convert {reader.Value} to DateOnly");
        }
    }
}
