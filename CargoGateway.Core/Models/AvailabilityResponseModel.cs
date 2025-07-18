namespace CargoGateway.Core.Models;

public class AvailabilityResponseModel
{
    public List<Shipment> Shipments { get; set; } = new();
}

public class Shipment
{
    public string CarrierCode { get; set; } = default!;
    public string FlightNumber { get; set; } = default!;
    public string CargoType { get; set; } = default!;
    public List<Leg> Legs { get; set; } = new();
}

public class Leg
{
    public string DepartureLocation { get; set; } = default!;
    public string ArrivalLocation { get; set; } = default!;
    public DateOnly DepartureDate { get; set; }
    public TimeOnly DepartureTime { get; set; }
    public DateOnly ArrivalDate { get; set; }
    public TimeOnly ArrivalTime { get; set; }
}