using CargoGateway.Application.DTO;

namespace CargoGateway.Application.Interfaces;

public interface ICargoService
{
    Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request);
}