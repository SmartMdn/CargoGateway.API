using CargoGateway.Core.Models;

namespace CargoGateway.Core.Interfaces;

public interface ICargoService
{
    Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request);
}