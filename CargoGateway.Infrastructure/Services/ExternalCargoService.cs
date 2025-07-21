using System.Net.Http.Json;
using Cargo.Libraries.Logistics.Models.Entities;
using Newtonsoft.Json;
using CargoGateway.Core.Exceptions;
using CargoGateway.Core.Interfaces;
using CargoGateway.Core.Models;
using CargoGateway.Core.Models.Request;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Infrastructure.Services;

public class ExternalCargoService(HttpClient httpClient, ISearchRepository repository, ILogger<ExternalCargoService> logger) : ICargoService
{
    // Настройка времени жизни кэша - данные актуальны в течение 15 минут
    private static readonly TimeSpan CacheMaxAge = TimeSpan.FromMinutes(15);

    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        // Сначала проверяем кэш в базе данных
        logger.LogInformation("Checking cache for search: {From} -> {To} on {Date}", 
            request.From, request.To, request.DateString);
        
        var cachedResult = await repository.FindRecentSearchAsync(
            request.From, request.To, request.Date, CacheMaxAge);
        
        if (cachedResult != null)
        {
            logger.LogInformation("Cache hit! Found recent search result from {CreatedAt}", 
                cachedResult.CreatedAtUtc);
            return MapToResponseModel(cachedResult);
        }
        
        logger.LogInformation("Cache miss. Sending request to external cargo service...");
        var response = await httpClient.PostAsJsonAsync("availability/search", new 
        {
            from = request.From,
            to = request.To,
            date = request.DateString // Используем строковое представление
        });
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
            throw new CargoServiceException(
                $"Failed to search for cargo availability. Status: {response.StatusCode}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        logger.LogDebug("Received response: {Content}", content);
        
        if (string.IsNullOrWhiteSpace(content))
            throw new CargoServiceException("Empty response from cargo service");

        try
        {
            var availability = JsonConvert.DeserializeObject<AvailabilityResponseModel>(content);
            if (availability == null)
                throw new CargoServiceException("Deserialization returned null object");
            if (availability.Shipments == null)
                throw new CargoServiceException("Deserialized object has null Shipments collection");
            logger.LogInformation("Successfully deserialized {Count} shipments", availability.Shipments.Count);
            var searchEntity = MapToSearchEntity(request, availability);
            await repository.AddSearchResultAsync(searchEntity);
            return availability;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON deserialization error: {Message}", ex.Message);
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

    private static AvailabilityResponseModel MapToResponseModel(SearchEntity searchEntity)
    {
        return new AvailabilityResponseModel
        {
            Shipments = searchEntity.Shipments.Select(sh => new Shipment
            {
                CarrierCode = sh.CarrierCode,
                FlightNumber = sh.FlightNumber,
                CargoType = sh.CargoType,
                Legs = sh.Legs.Select(leg => new Leg
                {
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