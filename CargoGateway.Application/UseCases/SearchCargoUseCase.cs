using CargoGateway.Application.DTO;
using CargoGateway.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Application.UseCases;

public class SearchCargoUseCase(
    ICacheService cacheService,
    IExternalCargoClient externalCargoClient,
    ICargoMapper mapper,
    ILogger<SearchCargoUseCase> logger)
    : ISearchCargoUseCase
{
    public async Task<AvailabilityResponseModel> ExecuteAsync(AvailabilitySearchRequest request)
    {
        logger.LogInformation("Executing cargo search: {From} -> {To} on {Date}",
            request.From, request.To, request.Date);

        // Step 1: Check cache
        var cachedResult = await cacheService.GetCachedSearchAsync(
            request.From, 
            request.To, 
            request.Date);

        if (cachedResult != null)
        {
            logger.LogInformation("Returning cached result");
            return mapper.MapToResponseModel(cachedResult);
        }

        // Step 2: Fetch from external API
        logger.LogInformation("Fetching from external cargo API");
        var availability = await externalCargoClient.SearchAvailabilityAsync(request);

        // Step 3: Save to cache for future requests
        var searchEntity = mapper.MapToSearchEntity(request, availability);
        await cacheService.SaveSearchAsync(searchEntity);

        logger.LogInformation("Search completed successfully");
        return availability;
    }
}
