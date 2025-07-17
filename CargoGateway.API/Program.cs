using CargoGateway.API.Persistence;
using CargoGateway.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =  Environment.GetEnvironmentVariable("CONNECTION_STRING") ??
                        builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string for PostgreSQL not found in environment variables.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<CargoService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/"); //for localhost deployment
    //client.BaseAddress = new Uri("http://fakeapi/"); // for docker deployment
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapControllers();

app.Run();