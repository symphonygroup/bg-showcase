namespace Cookbook.Shared.Routing;

public interface IRequestRoutingService
{
    Task<RouteResult> RouteRequest(string? routingKey);
}