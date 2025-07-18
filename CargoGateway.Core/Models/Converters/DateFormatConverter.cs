using Newtonsoft.Json.Converters;

namespace CargoGateway.Core.Models.Converters;

public class DateFormatConverter : IsoDateTimeConverter
{
    public DateFormatConverter() 
    {
        DateTimeFormat = "yyyy-MM-dd";
    }
}