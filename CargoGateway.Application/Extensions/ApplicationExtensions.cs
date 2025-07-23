using CargoGateway.Application.Interfaces;
using CargoGateway.Application.Mapping;
using CargoGateway.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CargoGateway.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICargoService, CargoService>();
        services.AddScoped<ICargoMapper, CargoMapper>();
        
        return services;
    }

}