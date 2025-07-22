using Cargo.Libraries.Logistics.Models.Entities;
using CargoGateway.Application.DTO;

namespace CargoGateway.Application.Interfaces;

public interface ICargoMapper
{
    SearchEntity MapToSearchEntity(AvailabilitySearchRequest request, AvailabilityResponseModel availability);
    AvailabilityResponseModel MapToResponseModel(SearchEntity searchEntity);
}
