using CargoGateway.Application.DTO;
using CargoGateway.Application.Interfaces;
using CargoGateway.Domain.Abstractions;
using CargoGateway.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Application.Services;

public class CargoService : ICargoService
{
    private readonly ISearchRepository _repository;
    private readonly IExternalCargoClient _externalCargoClient;
    private readonly ICargoMapper _mapper;
    private readonly ILogger<CargoService> _logger;
    private readonly ICachePolicy _cachePolicy;

    public CargoService(
        ISearchRepository repository,
        IExternalCargoClient externalClient,
        ICargoMapper mapper,
        ILogger<CargoService> logger)
    {
        _repository = repository;
        _externalCargoClient = externalClient;
        _mapper = mapper;
        _logger = logger;
        _cachePolicy = new FixedTimeCachePolicy(TimeSpan.FromMinutes(15));
    }

    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        _logger.LogInformation("Searching cargo: {From} -> {To} on {Date}",
            request.From, request.To, request.Date);
        
        var specification = new RecentSearchSpecification(_cachePolicy);
        
        var cachedResult = await _repository.FindBySpecificationAsync(specification,
            request.From,
            request.To,
            request.Date);

        if (cachedResult != null)
        {
            _logger.LogInformation("Cache hit from {CreatedAt}", cachedResult.CreatedAtUtc);
            return _mapper.MapToResponseModel(cachedResult);
        }
        
        _logger.LogInformation("Cache miss - calling external API");
        var availability = await _externalCargoClient.SearchAvailabilityAsync(request);
        
        var searchEntity = _mapper.MapToSearchEntity(request, availability);
        await _repository.SaveAsync(searchEntity);

        return availability;
    }
    
}