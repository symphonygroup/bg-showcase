using Cookbook.Api.Components;
using Cookbook.Shared;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Api.Application.Extensions;

public static class MassTransitConfigurationExtensions
{
    public static IServiceCollection ConfigureMassTransitWithRabbit(this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(ApiComponentsAssembly).Assembly;

        services.AddMassTransit(x =>
        {
            x.AddConsumers(assembly);

            x.ConfigureMassTransit(cfg => { cfg.AutoStart = true; });
        });

        services.AddOptions<MassTransitHostOptions>().Configure(options => { options.WaitUntilStarted = true; });
        services.Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMqTransport"));
        return services;
    }
}