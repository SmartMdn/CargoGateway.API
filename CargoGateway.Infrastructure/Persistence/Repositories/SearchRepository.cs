using Cargo.Libraries.Logistics.Models.Models;
using CargoGateway.Domain.Abstractions;
using CargoGateway.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CargoGateway.Infrastructure.Persistence.Repositories;

public class SearchRepository(ApplicationDbContext db) : ISearchRepository
{
    public async Task<Search?> FindBySpecificationAsync(
        ISearchSpecification specification,
        string from,
        string to,
        DateOnly date)
    {
        var query = db.SearchEntities
            .Include(s => s.Shipments)
            .ThenInclude(sh => sh.Legs)
            .Where(s =>
                s.From == from &&
                s.To == to &&
                s.Date == date);

        // Apply cache policy filtering at database level for RecentSearchSpecification
        if (specification is RecentSearchSpecification recentSpec)
        {
            var cutoffTime = DateTime.UtcNow - recentSpec.MaxAge;
            query = query.Where(s => s.CreatedAtUtc >= cutoffTime);
        }

        // Single database call with optimal filtering
        return await query
            .OrderByDescending(s => s.CreatedAtUtc)
            .FirstOrDefaultAsync();
    }

    public async Task SaveAsync(Search search)
    {
        db.SearchEntities.Add(search);
        await db.SaveChangesAsync();
    }
}
