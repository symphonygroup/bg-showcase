namespace Cookbook.Shared.Routing;

public interface IRequestRoutingCandidate
{
    Task<RouteResult?> IsValidCandidate(RequestRoutingCriteria criteria);
}