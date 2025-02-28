using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared;

public static class ServiceCollectionExtensions
{
    public static void AddMyOpenTelemetry(this IServiceCollection services, string serviceName,
        IConfiguration configuration)
    {
        var openTelemetryAutoInstrumentation =
            configuration.GetValue<bool>("OpenTelemetry:AutoInstrumentation");
        if (!openTelemetryAutoInstrumentation)
        {
            var openTelemetryUri =
                configuration.GetValue<string>("OpenTelemetry:Grpc") ?? "http://localhost:4316";

            services.AddOpenTelemetry()
                .ConfigureResource(r => r
                    .AddService(serviceName: serviceName, serviceInstanceId: Environment.MachineName)
                )
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetSampler(new AlwaysOnSampler())
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource(DiagnosticHeaders.DefaultListenerName)
                        .AddAWSInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter(opts =>
                        {
                            opts.Endpoint = new Uri(openTelemetryUri); // OTLP gRPC endpoint from docker-compose
                        });
                })
                .WithMetrics(metricsProviderBuilder =>
                {
                    metricsProviderBuilder
                        .SetExemplarFilter(ExemplarFilterType.AlwaysOn)
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddAWSInstrumentation()
                        .AddNpgsqlInstrumentation()
                        .AddMeter(InstrumentationOptions.MeterName)
                        .AddOtlpExporter(opts =>
                        {
                            opts.Endpoint = new Uri(openTelemetryUri); // OTLP gRPC endpoint from docker-compose
                        });
                });
        }
    }

    public static void AddMyTransport(this IBusRegistrationConfigurator config, IConfiguration configuration)
    {
        var isDockerCompose = configuration.GetValue<bool?>("IS_DOCKER_COMPOSE") ?? false;
        config.UsingAmazonSqs((mtContext, mtConfig) =>
        {
            mtConfig.Host(configuration.GetValue<string>("AWS_DEFAULT_REGION"), h =>
            {
                if (isDockerCompose)
                {
                    h.AccessKey(configuration.GetValue<string>("AWS_ACCESS_KEY_ID"));
                    h.SecretKey(configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY"));
                }
            });
            mtConfig.ConfigureEndpoints(mtContext);
        });
    }
}