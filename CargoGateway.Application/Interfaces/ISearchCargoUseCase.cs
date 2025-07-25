using CargoGateway.Application.DTO;

namespace CargoGateway.Application.Interfaces;

public interface ISearchCargoUseCase
{
    Task<AvailabilityResponseModel> ExecuteAsync(AvailabilitySearchRequest request);
}
