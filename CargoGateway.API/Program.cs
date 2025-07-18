using CargoGateway.Core.Extensions;
using CargoGateway.Infrastructure.Extensions;
using CargoGateway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Добавляем Newtonsoft.Json для контроллеров
builder.Services.AddControllers()
    .AddNewtonsoftJson(options => 
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
        
    });

// Регистрация сервисов
builder.Services
    .AddCoreServices()
    .AddInfrastructure(builder.Configuration);

// Логирование
builder.Services.AddLogging(logging => 
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Автоматические миграции
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