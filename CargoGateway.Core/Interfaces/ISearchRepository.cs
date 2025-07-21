using Cargo.Libraries.Logistics.Models.Entities;

namespace CargoGateway.Core.Interfaces;

public interface ISearchRepository
{
    Task AddSearchResultAsync(SearchEntity search);
    Task<SearchEntity?> FindRecentSearchAsync(string from, string to, DateOnly date, TimeSpan maxAge);
}