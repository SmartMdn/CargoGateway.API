namespace CargoGateway.Domain.Abstractions;

public interface ISearchSpecification
{
    bool IsSatisfiedBy(string from, string to, DateOnly date, DateTime createdAtUtc);
}

public class RecentSearchSpecification : ISearchSpecification
{
    private readonly ICachePolicy _cachePolicy;
    
    public TimeSpan MaxAge => _cachePolicy.MaxAge;
    
    public RecentSearchSpecification(ICachePolicy cachePolicy)
    {
        _cachePolicy = cachePolicy;
    }

    public bool IsSatisfiedBy(string from, string to, DateOnly date, DateTime createdAtUtc)
    {
        return _cachePolicy.IsValid(createdAtUtc);
    }
}