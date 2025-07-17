namespace CargoGateway.API.Models;

public class AvailabilitySearchRequest
{
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
    public DateOnly Date { get; set; }
}