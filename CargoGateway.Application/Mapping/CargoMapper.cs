using Cargo.Libraries.Logistics.Models.Models;
using CargoGateway.Application.DTO;
using CargoGateway.Application.Interfaces;

namespace CargoGateway.Application.Mapping;

public class CargoMapper : ICargoMapper
{
    public Search MapToSearchEntity(AvailabilitySearchRequest request, AvailabilityResponseModel availability) =>
        new()
        {
            Id = Guid.NewGuid(),
            From = request.From,
            To = request.To,
            Date = request.Date,
            CreatedAtUtc = DateTime.UtcNow,
            Shipments = availability.Shipments.Select(MapToShipmentEntity).ToList()
        };

    public AvailabilityResponseModel MapToResponseModel(Search searchEntity) =>
        new()
        {
            Shipments = searchEntity.Shipments.Select(MapToShipment).ToList()
        };

    private Shipment MapToShipmentEntity(ShipmentDTO shipmentDto) =>
        new()
        {
            Id = Guid.NewGuid(),
            CarrierCode = shipmentDto.CarrierCode,
            FlightNumber = shipmentDto.FlightNumber,
            CargoType = shipmentDto.CargoType,
            Legs = shipmentDto.Legs.Select(MapToLegEntity).ToList()
        };

    private Leg MapToLegEntity(LegDTO legDto) =>
        new()
        {
            Id = Guid.NewGuid(),
            DepartureLocation = legDto.DepartureLocation,
            ArrivalLocation = legDto.ArrivalLocation,
            DepartureDate = legDto.DepartureDate,
            DepartureTime = legDto.DepartureTime,
            ArrivalDate = legDto.ArrivalDate,
            ArrivalTime = legDto.ArrivalTime
        };

    private ShipmentDTO MapToShipment(Shipment shipmentEntity) =>
        new()
        {
            CarrierCode = shipmentEntity.CarrierCode,
            FlightNumber = shipmentEntity.FlightNumber,
            CargoType = shipmentEntity.CargoType,
            Legs = shipmentEntity.Legs.Select(MapToLeg).ToList()
        };

    private LegDTO MapToLeg(Leg legEntity) =>
        new()
        {
            DepartureLocation = legEntity.DepartureLocation,
            ArrivalLocation = legEntity.ArrivalLocation,
            DepartureDate = legEntity.DepartureDate,
            DepartureTime = legEntity.DepartureTime,
            ArrivalDate = legEntity.ArrivalDate,
            ArrivalTime = legEntity.ArrivalTime
        };
}
