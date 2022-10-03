using Cookbook.Inventory.Components.Ingredients.Persistence;
using Cookbook.Shared;
using Cookbook.Shared.Configuration;
using Cookbook.Shared.Extensions;
using Cookbook.Shared.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Inventory.Application.Extensions;

public static class ServicesConfigurationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRequestRoutingService, RequestRoutingService>();
        services.ConfigureAutoMapper();
        services.AddRequestRoutingCandidates();
        services.AddReceiveEndpointOptions(configuration);
        services.AddControllers();

        return services;
    }
}