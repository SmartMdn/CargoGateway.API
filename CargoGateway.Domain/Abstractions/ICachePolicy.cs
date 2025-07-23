namespace CargoGateway.Domain.Abstractions;

public interface ICachePolicy
{
    bool IsValid(DateTime createdAtUtc);
    TimeSpan MaxAge { get; }
}

public class FixedTimeCachePolicy(TimeSpan maxAge) : ICachePolicy
{
    public TimeSpan MaxAge { get; } = maxAge;

    public bool IsValid(DateTime createdAtUtc)
    {
        return DateTime.UtcNow - createdAtUtc <= MaxAge;
    }

}