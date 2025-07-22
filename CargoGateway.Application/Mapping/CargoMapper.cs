using Cargo.Libraries.Logistics.Models.Entities;
using CargoGateway.Application.DTO;
using CargoGateway.Application.Interfaces;

namespace CargoGateway.Application.Mapping;

public class CargoMapper : ICargoMapper
{
    public SearchEntity MapToSearchEntity(AvailabilitySearchRequest request, AvailabilityResponseModel availability) =>
        new()
        {
            Id = Guid.NewGuid(),
            From = request.From,
            To = request.To,
            Date = request.Date,
            CreatedAtUtc = DateTime.UtcNow,
            Shipments = availability.Shipments.Select(MapToShipmentEntity).ToList()
        };

    public AvailabilityResponseModel MapToResponseModel(SearchEntity searchEntity) =>
        new()
        {
            Shipments = searchEntity.Shipments.Select(MapToShipment).ToList()
        };

    private ShipmentEntity MapToShipmentEntity(Shipment shipment) =>
        new()
        {
            Id = Guid.NewGuid(),
            CarrierCode = shipment.CarrierCode,
            FlightNumber = shipment.FlightNumber,
            CargoType = shipment.CargoType,
            Legs = shipment.Legs.Select(MapToLegEntity).ToList()
        };

    private LegEntity MapToLegEntity(Leg leg) =>
        new()
        {
            Id = Guid.NewGuid(),
            DepartureLocation = leg.DepartureLocation,
            ArrivalLocation = leg.ArrivalLocation,
            DepartureDate = leg.DepartureDate,
            DepartureTime = leg.DepartureTime,
            ArrivalDate = leg.ArrivalDate,
            ArrivalTime = leg.ArrivalTime
        };

    private Shipment MapToShipment(ShipmentEntity shipmentEntity) =>
        new()
        {
            CarrierCode = shipmentEntity.CarrierCode,
            FlightNumber = shipmentEntity.FlightNumber,
            CargoType = shipmentEntity.CargoType,
            Legs = shipmentEntity.Legs.Select(MapToLeg).ToList()
        };

    private Leg MapToLeg(LegEntity legEntity) =>
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
