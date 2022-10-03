using Cookbook.Cooking.Application.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Cooking.Application.Extensions;

public static class AutoMapperConfigurationExtensions
{
    public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
    {
        var assembly = typeof(ProfileAssembly).Assembly;
        services.AddAutoMapper(assembly);

        return services;
    }
}