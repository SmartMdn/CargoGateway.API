using System.Net.Http.Json;
using Cargo.Libraries.Logistics.Models.Interfaces;
using CargoGateway.Application.DTO;
using CargoGateway.Application.Exceptions;
using CargoGateway.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Infrastructure.Services;

public class ExternalCargoClient(HttpClient httpClient, ILogger<ExternalCargoClient> logger) : IExternalCargoClient
{
    public async Task<AvailabilityResponseModel> SearchAvailabilityAsync(AvailabilitySearchRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("availability/search", new 
        {
            from = request.From,
            to = request.To,
            date = request.Date
        });
        var content = response.Content;
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await content.ReadAsStringAsync();
            
            logger.LogError("API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
            
            throw new CargoServiceException($"API call failed with status: {response.StatusCode}");
        }
        
        try
        {
            var availability = await content.ReadFromJsonAsync<AvailabilityResponseModel>() 
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