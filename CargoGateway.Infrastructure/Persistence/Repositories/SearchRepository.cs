using Cargo.Libraries.Logistics.Models.Models;
using CargoGateway.Domain.Abstractions;
using CargoGateway.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CargoGateway.Infrastructure.Persistence.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly ApplicationDbContext _db;

    public SearchRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Search?> FindBySpecificationAsync(
        ISearchSpecification specification,
        string from,
        string to,
        DateOnly date)
    {
        var query = await _db.SearchEntities
            .Include(s=> s.Shipments)
                .ThenInclude(sh => sh.Legs)
            .Where(s=>
                s.From == from &&
                s.To == to &&
                s.Date == date)
            .ToListAsync();
        
        return query.FirstOrDefault(s => specification.IsSatisfiedBy(from, to, date, s.CreatedAtUtc));
    }

    public async Task SaveAsync(Search search)
    {
        _db.SearchEntities.Add(search);
        await _db.SaveChangesAsync();
    }
    
}
