using CargoGateway.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CargoGateway.Domain.Extensions;

public static class DomainExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICachePolicy>(_ => 
            new FixedTimeCachePolicy(TimeSpan.FromMinutes(15)));
            
        return services;
    }

}