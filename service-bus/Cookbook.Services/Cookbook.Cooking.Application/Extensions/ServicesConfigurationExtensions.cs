using Cookbook.Shared;
using Cookbook.Shared.Extensions;
using Cookbook.Shared.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Cooking.Application.Extensions;

public static class ServicesConfigurationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRequestRoutingService, RequestRoutingService>();
        services.ConfigureAutoMapper();
        services.AddRequestRoutingCandidates();
        services.AddReceiveEndpointOptions(configuration);
        services.AddCors(options =>
        {
            options.AddPolicy("AllowClient", builder => builder.WithOrigins("https://localhost:7134").AllowAnyHeader().AllowAnyMethod());
        });
        services.AddControllers();
        return services;
    }
}