using CargoGateway.Core.DTO;

namespace CargoGateway.Core.Interfaces;

public interface ICargoService
{
    Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request);
}