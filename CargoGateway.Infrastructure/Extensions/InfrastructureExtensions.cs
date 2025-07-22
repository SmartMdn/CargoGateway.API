using Cargo.Libraries.Logistics.Models.Interfaces;
using CargoGateway.Application.Interfaces;
using CargoGateway.Application.Mapping;
using CargoGateway.Infrastructure.Persistence;
using CargoGateway.Infrastructure.Persistence.Repositories;
using CargoGateway.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CargoGateway.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                               ?? throw new InvalidOperationException("Connection string not found");
        
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(connectionString));
        
        services.AddScoped<ISearchRepository, SearchRepository>();
        
        services.AddHttpClient<ICargoService, ExternalCargoService>((provider, client) => 
        {
            var config = provider.GetRequiredService<IConfiguration>();
            client.BaseAddress = new Uri(config["CargoApi:BaseUrl"] ?? "http://localhost:5002/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        return services;
    }
}