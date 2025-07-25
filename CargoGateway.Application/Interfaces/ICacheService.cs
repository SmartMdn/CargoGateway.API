using Cargo.Libraries.Logistics.Models.Models;

namespace CargoGateway.Application.Interfaces;

public interface ICacheService
{
    Task<Search?> GetCachedSearchAsync(string from, string to, DateOnly date);
    Task SaveSearchAsync(Search searchEntity);
}
