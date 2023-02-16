﻿using Cookbook.Shared;
using Cookbook.Shared.Extensions;
using Cookbook.Shared.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Api.Application.Extensions;

public static class ServicesConfigurationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IRequestRoutingService, RequestRoutingService>();
        services.AddRequestRoutingCandidates();
        services.AddReceiveEndpointOptions(configuration);
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowClient", builder => builder.WithOrigins("https://localhost:7048", "https://localhost:7167").AllowAnyHeader().AllowAnyMethod());
        });

        return services;
    }
}