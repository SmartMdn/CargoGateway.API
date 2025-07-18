using Newtonsoft.Json.Converters;

namespace CargoGateway.Core.Models.Converters;

public class TimeFormatConverter : IsoDateTimeConverter
{
    public TimeFormatConverter()
    {
        DateTimeFormat = "HH:mm:ss";
    }
}