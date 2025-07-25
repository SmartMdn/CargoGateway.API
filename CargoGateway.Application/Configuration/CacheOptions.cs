namespace CargoGateway.Application.Configuration;

public class CacheOptions
{
    public const string SectionName = "Cache";
    
    public TimeSpan DefaultCacheDuration { get; set; } = TimeSpan.FromMinutes(15);
}
