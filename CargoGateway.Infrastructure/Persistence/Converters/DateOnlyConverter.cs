using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CargoGateway.Infrastructure.Persistence.Converters;

public class DateOnlyConverter() : ValueConverter<DateOnly, DateTime>(
    d => d.ToDateTime(TimeOnly.MinValue),
    d => DateOnly.FromDateTime(d));