using System.Net.Http.Json;
using System.Text.Json;
using Cargo.Libraries.Logistics.Models.Entities;
using CargoGateway.Core.Exceptions;
using CargoGateway.Core.Interfaces;
using CargoGateway.Core.Models;

namespace CargoGateway.Infrastructure.Services;

public class ExternalCargoService(HttpClient httpClient, ISearchRepository repository) : ICargoService
{
    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("availability/search", request);
    
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new CargoServiceException(
                $"Failed to search for cargo availability. Status: {response.StatusCode}. Response: {errorContent}");
        }
        
        // Добавить логирование ошибок
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
            throw new Exception("Empty response from cargo service");
    
        try
        {
            var availability = JsonSerializer.Deserialize<AvailabilityResponseModel>(content);
            if (availability is null) throw new Exception("Deserialization failed");
        
            var searchEntity = MapToSearchEntity(request, availability);
            await repository.AddSearchResultAsync(searchEntity);
        
            return availability;
        }
        catch (JsonException ex)
        {
            throw new Exception($"JSON error: {ex.Message}\nResponse: {content}");
        }
    }

    private static SearchEntity MapToSearchEntity(AvailabilitySearchRequest request, AvailabilityResponseModel availability)
    {
        return new SearchEntity
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
    }
}