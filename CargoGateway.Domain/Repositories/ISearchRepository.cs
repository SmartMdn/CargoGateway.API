using Cargo.Libraries.Logistics.Models.Models;
using CargoGateway.Domain.Abstractions;

namespace CargoGateway.Domain.Repositories;

public interface ISearchRepository
{
    Task<Search?> FindBySpecificationAsync(ISearchSpecification specification,
        string from,
        string to,
        DateOnly date);
    
    Task SaveAsync(Search search);
}