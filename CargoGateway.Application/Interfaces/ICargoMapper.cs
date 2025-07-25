using Cargo.Libraries.Logistics.Models.Models;
using CargoGateway.Application.DTO;

namespace CargoGateway.Application.Interfaces;

public interface ICargoMapper
{
    Search MapToSearchEntity(AvailabilitySearchRequest request, AvailabilityResponseModel availability);
    AvailabilityResponseModel MapToResponseModel(Search searchEntity);
}
