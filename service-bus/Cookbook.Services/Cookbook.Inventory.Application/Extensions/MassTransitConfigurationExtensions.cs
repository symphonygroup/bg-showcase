using Cookbook.Inventory.Components;
using Cookbook.Shared;
using Cookbook.Shared.Configuration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Inventory.Application.Extensions;

public static class MassTransitConfigurationExtensions
{
    public static IServiceCollection ConfigureMassTransitWithRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMqTransport"));
        var mongoDbDatabaseOptions = configuration.GetSection("MongoDbDatabase").Get<MongoDbDatabaseOptions>();
        var mongoDbCollections = configuration.GetSection("MongoDbCollections").Get<MongoDbCollectionsOptions>();
        
        services.AddMassTransit(x =>
        {
            var assembly = typeof(ComponentsAssembly).Assembly;
            
            x.AddConsumers(assembly);
            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddActivities(assembly);

            x.ConfigureMassTransit();
        });

        services.AddOptions<MassTransitHostOptions>().Configure(options => { options.WaitUntilStarted = true; });

        return services;
    }
}