using System.Net.Http.Json;
using Cargo.Libraries.Logistics.Models.Entities;
using Newtonsoft.Json;
using CargoGateway.Core.Exceptions;
using CargoGateway.Core.Interfaces;
using CargoGateway.Core.Models;
using CargoGateway.Core.Models.Request;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Infrastructure.Services;

public class ExternalCargoService : ICargoService
{
    private readonly HttpClient _httpClient;
    private readonly ISearchRepository _repository;
    private readonly ILogger<ExternalCargoService> _logger;
    
    public ExternalCargoService(
        HttpClient httpClient, 
        ISearchRepository repository,
        ILogger<ExternalCargoService> logger)
    {
        _httpClient = httpClient;
        _repository = repository;
        _logger = logger;
    }

    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        _logger.LogInformation("Sending request to external cargo service...");
        var response = await _httpClient.PostAsJsonAsync("availability/search", new 
        {
            from = request.From,
            to = request.To,
            date = request.DateString // Используем строковое представление
        });
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
            throw new CargoServiceException(
                $"Failed to search for cargo availability. Status: {response.StatusCode}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Received response: {Content}", content);
        
        if (string.IsNullOrWhiteSpace(content))
            throw new CargoServiceException("Empty response from cargo service");

        try
        {
            var availability = JsonConvert.DeserializeObject<AvailabilityResponseModel>(content);
            if (availability == null)
                throw new CargoServiceException("Deserialization returned null object");
            if (availability.Shipments == null)
                throw new CargoServiceException("Deserialized object has null Shipments collection");
            _logger.LogInformation("Successfully deserialized {Count} shipments", availability.Shipments.Count);
            var searchEntity = MapToSearchEntity(request, availability);
            await _repository.AddSearchResultAsync(searchEntity);
            return availability;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error: {Message}", ex.Message);
            throw new CargoServiceException($"JSON error: {ex.Message}\nResponse: {content}", ex);
        }
    }

    private static SearchEntity MapToSearchEntity(AvailabilitySearchRequest request, AvailabilityResponseModel availability)
    {
        return new SearchEntity
        {
            Id = Guid.NewGuid(),
            From = request.From,
            To = request.To,
            Date = request.Date, // Используем DateOnly свойство
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