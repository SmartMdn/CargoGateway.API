using System.Net.Http.Json;
using Cargo.Libraries.Logistics.Models.Entities;
using CargoGateway.Core.Interfaces;
using CargoGateway.Core.Models;
using CargoGateway.Infrastructure.Persistence.Repositories;

namespace CargoGateway.Infrastructure.Services;

public class ExternalCargoService(HttpClient httpClient, SearchRepository repository) : ICargoService
{
    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("availability/search", request);
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to search for cargo availability.");
        
        var availability = await response.Content.ReadFromJsonAsync<AvailabilityResponseModel>();
        if (availability is null)
            throw new Exception("Failed to deserialize cargo availability.");
        
        var searchEntity = new SearchEntity
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

        await repository.AddSearchResultAsync(searchEntity);
        return availability;
    }
}