using CargoGateway.Application.DTO;
using CargoGateway.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CargoGateway.Application.Services;

public class CargoService(
    ISearchCargoUseCase searchCargoUseCase,
    ILogger<CargoService> logger)
    : ICargoService
{
    public async Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request)
    {
        logger.LogInformation("CargoService: Delegating search request to use case");
        
        return await searchCargoUseCase.ExecuteAsync(request);
    }
}