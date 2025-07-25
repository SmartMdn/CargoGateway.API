using CargoGateway.Application.Configuration;
using CargoGateway.Application.Interfaces;
using CargoGateway.Application.Mapping;
using CargoGateway.Application.Services;
using CargoGateway.Application.UseCases;
using CargoGateway.Domain.Abstractions;
using CargoGateway.Domain.Repositories;
using CargoGateway.Infrastructure.Persistence;
using CargoGateway.Infrastructure.Persistence.Repositories;
using CargoGateway.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure cache options
builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection(CacheOptions.SectionName));

// Application services
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ICargoMapper, CargoMapper>();

// Use Cases
builder.Services.AddScoped<ISearchCargoUseCase, SearchCargoUseCase>();

// Cache policy with configuration
builder.Services.AddSingleton<ICachePolicy>(provider =>
{
    var cacheOptions = builder.Configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>() 
                      ?? new CacheOptions();
    return new FixedTimeCachePolicy(cacheOptions.DefaultCacheDuration);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string not found in configuration");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddHttpClient<IExternalCargoClient, ExternalCargoClient>((provider, client) =>
{
    var baseUrl = builder.Configuration["CargoApi:BaseUrl"]
                  ?? throw new InvalidOperationException("CargoApi:BaseUrl is not configured");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<ISearchRepository, SearchRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
}

app.MapControllers();
app.Run();