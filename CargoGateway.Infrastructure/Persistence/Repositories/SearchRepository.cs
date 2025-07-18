using Cargo.Libraries.Logistics.Models.Entities;
using CargoGateway.Core.Interfaces;

namespace CargoGateway.Infrastructure.Persistence.Repositories;

public class SearchRepository(ApplicationDbContext db) : ISearchRepository
{
    public async Task AddSearchResultAsync(SearchEntity search)
    {
        db.SearchEntities.Add(search);
        await db.SaveChangesAsync();
    }
}
