using Cookbook.Shared.Configuration;

namespace Cookbook.Cooking.Application.Extensions;

using Components;
using Shared;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cookbook.Cooking.Components.Cooking;

public static class MassTransitConfigurationExtensions
{
    public static IServiceCollection ConfigureMassTransitWithRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMqTransport"));

        var mongoDbConfiguration = configuration.GetSection("MongoDbDatabase").Get<MongoDbDatabaseOptions>();
        var mongoDbCollections = configuration.GetSection("MongoDbCollections").Get<MongoDbCollectionOptions>();

        services.AddMassTransit(x =>
        {
            var assembly = typeof(ComponentsAssembly).Assembly;

            x.AddConsumers(assembly);

            x.AddSagaStateMachine<CookingStateMachine, CookingState>(typeof(CookingStateMachineDefinition))
                .MongoDbRepository(r =>
                {
                    r.Connection = mongoDbConfiguration.ConnectionString;
                    r.DatabaseName = mongoDbConfiguration.DatabaseName;
                    r.CollectionName = mongoDbCollections.CookingStates;
                });

            x.AddActivities(assembly);

            x.ConfigureMassTransit();
        });

        services.AddOptions<MassTransitHostOptions>().Configure(options => { options.WaitUntilStarted = true; });

        return services;
    }
}