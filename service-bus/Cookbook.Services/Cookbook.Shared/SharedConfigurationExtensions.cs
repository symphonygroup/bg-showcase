using Cookbook.Shared.Configuration;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Cookbook.Shared;

public static class SharedConfigurationExtensions
{
    static bool? _isRunningInContainer;

    public static bool IsRunningInContainer =>
        _isRunningInContainer ??=
            bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inContainer) &&
            inContainer;

    public static void ConfigureMassTransit(this IBusRegistrationConfigurator configurator,
        Action<IRabbitMqBusFactoryConfigurator> configure = null)
    {
        configurator.SetKebabCaseEndpointNameFormatter();
        configurator.AddPublishMessageScheduler();
        configurator.AddDelayedMessageScheduler();
        configurator.AddHangfireConsumers();

        configurator.UsingRabbitMq((context, cfg) =>
        {
            if (IsRunningInContainer)
                cfg.Host("rabbitmq");

            cfg.UsePublishMessageScheduler();
            cfg.UseDelayedMessageScheduler();

            configure?.Invoke(cfg);

            cfg.ConfigureEndpoints(context);
        });
    }

    public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireConfiguration = configuration.GetSection("Hangfire").Get<HangfireConfigurationOptions>();

        services.AddHangfire(config =>
        {
            config.UseMongoStorage(hangfireConfiguration.ConnectionString, new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                },
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
            });
        });
        services.AddHangfireServer();
        return services;
    }

    public static IApplicationBuilder UseConfiguredHangfire(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard();
        return app;
    }

    public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthCheckResponseWriter
        });
        app.UseHealthChecks("/health/live", new HealthCheckOptions { ResponseWriter = HealthCheckResponseWriter });

        return app;
    }

    static Task HealthCheckResponseWriter(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(result.ToJsonString());
    }
}