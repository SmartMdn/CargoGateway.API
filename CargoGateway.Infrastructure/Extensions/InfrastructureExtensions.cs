using CargoGateway.Application.Interfaces;
using CargoGateway.Domain.Repositories;
using CargoGateway.Infrastructure.Persistence;
using CargoGateway.Infrastructure.Persistence.Repositories;
using CargoGateway.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CargoGateway.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                               ?? throw new InvalidOperationException("Connection string not found");
        
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(connectionString));
        
        services.AddHttpClient<IExternalCargoClient, ExternalCargoClient>((provider, client) => 
        {
            var baseUrl = configuration["CargoApi:BaseUrl"] 
                          ?? throw new InvalidOperationException("CargoApi:BaseUrl is not configured");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<ISearchRepository, SearchRepository>();
        
        return services;
    }

}