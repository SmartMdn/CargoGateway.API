using System.Net.Http.Json;
using Cargo.Libraries.Logistics.Models.Interfaces;
using CargoGateway.Application.DTO;
using CargoGateway.Application.Exceptions;
using CargoGateway.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Infrastructure.Services;

public class ExternalCargoService(HttpClient httpClient, ISearchRepository repository, ILogger<ExternalCargoService> logger, ICargoMapper cargoMapper) : ICargoService
{
    private static readonly TimeSpan CacheMaxAge = TimeSpan.FromMinutes(15);

    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        logger.LogInformation("Searching cargo: {From} -> {To} on {Date}", request.From, request.To, request.Date);
        
        // Check cache first
        var cachedResult = await repository.FindRecentSearchAsync(request.From, request.To, request.Date, CacheMaxAge);
        if (cachedResult != null)
        {
            logger.LogInformation("Cache hit from {CreatedAt}", cachedResult.CreatedAtUtc);
            return cargoMapper.MapToResponseModel(cachedResult);
        }
        
        // Make external API call
        logger.LogInformation("Cache miss - calling external API");
        var availability = await CallExternalApiAsync(request);
        
        // Save to cache and return
        var searchEntity = cargoMapper.MapToSearchEntity(request, availability);
        await repository.AddSearchResultAsync(searchEntity);
        
        return availability;
    }

    private async Task<AvailabilityResponseModel> CallExternalApiAsync(AvailabilitySearchRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("availability/search", new 
        {
            from = request.From,
            to = request.To,
            date = request.Date
        });
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
            throw new CargoServiceException($"API call failed with status: {response.StatusCode}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrWhiteSpace(content))
            throw new CargoServiceException("Empty response from cargo service");

        try
        {
            var availability = await response.Content.ReadFromJsonAsync<AvailabilityResponseModel>() 
                ?? throw new CargoServiceException("Deserialization returned null");
            
            if (availability.Shipments == null)
                throw new CargoServiceException("Response missing shipments data");
            
            logger.LogInformation("Retrieved {Count} shipments", availability.Shipments.Count);
            return availability;
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new CargoServiceException("Failed to deserialize external API response", ex);
        }
    }
}