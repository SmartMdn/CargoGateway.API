using CargoGateway.Core.Models.Converters;
using Newtonsoft.Json;

namespace CargoGateway.Core.Models;

public class AvailabilityResponseModel
{
    [JsonProperty("shipments")]
    public List<Shipment> Shipments { get; set; } = new();
}

public class Shipment
{
    [JsonProperty("carrierCode")]
    public string CarrierCode { get; set; } = default!;
    
    [JsonProperty("flightNumber")]
    public string FlightNumber { get; set; } = default!;
    
    [JsonProperty("cargoType")]
    public string CargoType { get; set; } = default!;
    
    [JsonProperty("legs")]
    public List<Leg> Legs { get; set; } = new();
}

public class Leg
{
    [JsonProperty("departureLocation")]
    public string DepartureLocation { get; set; } = default!;
    
    [JsonProperty("arrivalLocation")]
    public string ArrivalLocation { get; set; } = default!;
    
    [JsonProperty("departureDate")]
    public string DepartureDateString { get; set; } = default!; 
    
    [JsonIgnore]
    public DateOnly DepartureDate
    {
        get => DateOnly.Parse(DepartureDateString);
        set => DepartureDateString = value.ToString("yyyy-MM-dd");
    }
    
    [JsonProperty("departureTime")]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly DepartureTime { get; set; }
    
    [JsonProperty("arrivalDate")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly ArrivalDate { get; set; }
    
    [JsonProperty("arrivalTime")]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly ArrivalTime { get; set; }
}