using Cargo.Libraries.Logistics.Models.DTO;
using Cargo.Libraries.Logistics.Models.Entities;
using CargoGateway.API.Models;
using CargoGateway.API.Persistence;

namespace CargoGateway.API.Services;

public class CargoService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CargoService> _logger;

    public CargoService(HttpClient httpClient, ApplicationDbContext db, ILogger<CargoService> logger)
    {
        _httpClient = httpClient;
        _db = db;
        _logger = logger;
    }

    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("availability/search", request);
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to search for cargo availability.");
        var rawJson = await response.Content.ReadAsStringAsync();
        // Временно залогируйте или выведите rawJson в консоль/лог
        _logger.LogInformation("Raw JSON: {RawJson}", rawJson);

        var availability = await response.Content.ReadFromJsonAsync<AvailabilityResponseModel>();
        if (availability is null)
            throw new Exception($"Failed to deserialize cargo availability. Raw response: {rawJson}");
       
        
        var search = new SearchEntity
        {
            Id = Guid.NewGuid(),
            From = request.From,
            To = request.To,
            Date = request.Date,
            CreatedAtUtc = DateTime.UtcNow,
            Shipments = availability.Shipments.Select(sh => new ShipmentEntity
            {
                Id = Guid.NewGuid(),
                CarrierCode = sh.CarrierCode,
                FlightNumber = sh.FlightNumber,
                CargoType = sh.CargoType,
                Legs = sh.Legs.Select(leg => new LegEntity
                {
                    Id = Guid.NewGuid(),
                    DepartureLocation = leg.DepartureLocation,
                    ArrivalLocation = leg.ArrivalLocation,
                    DepartureDate = leg.DepartureDate,
                    DepartureTime = leg.DepartureTime,
                    ArrivalDate = leg.ArrivalDate,
                    ArrivalTime = leg.ArrivalTime
                }).ToList()
            }).ToList()
        };
        
        _db.SearchEntities.Add(search);
        await _db.SaveChangesAsync();
        
        return availability;
    }
}