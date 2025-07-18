using CargoGateway.Core.Models;
using CargoGateway.Core.Models.Request;

namespace CargoGateway.Core.Interfaces;

public interface ICargoService
{
    Task<AvailabilityResponseModel> SearchAsync(AvailabilitySearchRequest request);
}