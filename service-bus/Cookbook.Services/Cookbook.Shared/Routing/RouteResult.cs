namespace Cookbook.Shared.Routing;

public record RouteResult
{
    public RouteDisposition Disposition { get; init; }

    public Uri? DestinationAddress { get; init; }
}