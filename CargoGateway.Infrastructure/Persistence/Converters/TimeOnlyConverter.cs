using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CargoGateway.Infrastructure.Persistence.Converters;

public class TimeOnlyConverter() : ValueConverter<TimeOnly, TimeSpan>(
    t => t.ToTimeSpan(),
    t => TimeOnly.FromTimeSpan(t));