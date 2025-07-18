using CargoGateway.Core.Interfaces;
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

        // Регистрация DbContext
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(connectionString));
        
        // Регистрация репозитория
        services.AddScoped<ISearchRepository, SearchRepository>();
        
        // Регистрация HttpClient с настройкой базового адреса
        services.AddHttpClient<ICargoService, ExternalCargoService>(client => 
        {
            client.BaseAddress = new Uri(configuration["CargoApi:BaseUrl"] 
                                         ?? "http://localhost:5002/");
        });
        
        return services;
    }
}