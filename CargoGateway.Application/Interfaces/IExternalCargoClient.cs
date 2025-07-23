using CargoGateway.Application.DTO;

namespace CargoGateway.Application.Interfaces;

public interface IExternalCargoClient
{
    Task<AvailabilityResponseModel> SearchAvailabilityAsync(AvailabilitySearchRequest request);
}
