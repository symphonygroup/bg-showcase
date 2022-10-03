namespace Cookbook.Shared.Configuration;

public record ReceiveEndpointOptions
{
    public int? PrefetchCount { get; init; }
    public int? ConcurrentMessageLimit { get; init; }
}