using Cargo.Libraries.Logistics.Models.Entities;
using CargoGateway.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CargoGateway.Infrastructure.Persistence.Repositories;

public class SearchRepository(ApplicationDbContext db) : ISearchRepository
{
    public async Task AddSearchResultAsync(SearchEntity search)
    {
        db.SearchEntities.Add(search);
        await db.SaveChangesAsync();
    }

    public async Task<SearchEntity?> FindRecentSearchAsync(string from, string to, DateOnly date, TimeSpan maxAge)
    {
        var cutoffTime = DateTime.UtcNow - maxAge;
        
        return await db.SearchEntities
            .Include(s => s.Shipments)
                .ThenInclude(sh => sh.Legs)
            .FirstOrDefaultAsync(s => 
                s.From == from && 
                s.To == to && 
                s.Date == date && 
                s.CreatedAtUtc >= cutoffTime);
    }
}
