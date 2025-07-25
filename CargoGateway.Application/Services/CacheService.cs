using Cargo.Libraries.Logistics.Models.Models;
using CargoGateway.Application.Interfaces;
using CargoGateway.Domain.Abstractions;
using CargoGateway.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Application.Services;

public class CacheService(
    ISearchRepository repository,
    ICachePolicy cachePolicy,
    ILogger<CacheService> logger)
    : ICacheService
{
    public async Task<Search?> GetCachedSearchAsync(string from, string to, DateOnly date)
    {
        logger.LogDebug("Checking cache for search: {From} -> {To} on {Date}", from, to, date);
        
        var specification = new RecentSearchSpecification(cachePolicy);
        
        var cachedResult = await repository.FindBySpecificationAsync(specification, from, to, date);
        
        if (cachedResult != null)
        {
            logger.LogInformation("Cache hit: Found cached search from {CreatedAt}", cachedResult.CreatedAtUtc);
            return cachedResult;
        }
        
        logger.LogDebug("Cache miss: No recent cached search found");
        return null;
    }

    public async Task SaveSearchAsync(Search searchEntity)
    {
        logger.LogDebug("Saving search to cache: {From} -> {To} on {Date}", 
            searchEntity.From, searchEntity.To, searchEntity.Date);
            
        await repository.SaveAsync(searchEntity);
        
        logger.LogInformation("Search saved to cache successfully");
    }
}