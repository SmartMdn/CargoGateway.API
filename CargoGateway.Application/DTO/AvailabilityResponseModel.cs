using Cargo.Libraries.Logistics.Models.Converters;
using System.Text.Json.Serialization;

namespace CargoGateway.Application.DTO;

public class AvailabilityResponseModel
{
    [JsonPropertyName("shipments")]
    public List<ShipmentDTO> Shipments { get; set; } = new();
}

public class ShipmentDTO
{
    [JsonPropertyName("carrierCode")]
    public string CarrierCode { get; set; } = default!;
    
    [JsonPropertyName("flightNumber")]
    public string FlightNumber { get; set; } = default!;
    
    [JsonPropertyName("cargoType")]
    public string CargoType { get; set; } = default!;
    
    [JsonPropertyName("legs")]
    public List<LegDTO> Legs { get; set; } = new();
}

public class LegDTO
{
    [JsonPropertyName("departureLocation")]
    public string DepartureLocation { get; set; } = default!;
    
    [JsonPropertyName("arrivalLocation")]
    public string ArrivalLocation { get; set; } = default!;
    
    [JsonPropertyName("departureDate")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly DepartureDate { get; set; }
    
    [JsonPropertyName("departureTime")]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly DepartureTime { get; set; }
    
    [JsonPropertyName("arrivalDate")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly ArrivalDate { get; set; }
    
    [JsonPropertyName("arrivalTime")]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly ArrivalTime { get; set; }
}