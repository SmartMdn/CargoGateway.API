using CargoGateway.Core.Extensions;
using CargoGateway.Infrastructure.Extensions;
using CargoGateway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCoreServices()       // Из Core
    .AddInfrastructure(builder.Configuration); // Из Infrastructure

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Автоматическое применение миграций
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();
app.Run();