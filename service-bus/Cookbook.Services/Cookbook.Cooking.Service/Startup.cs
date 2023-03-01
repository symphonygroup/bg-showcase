using Cookbook.Cooking.Application.Extensions;
using Cookbook.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cookbook.Cooking.Service;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureDatabase(Configuration);
        services.AddApplicationServices(Configuration);
        services.ConfigureHangfire(Configuration);
        services.ConfigureMassTransitWithRabbitMq(Configuration);
        services.AddOpenTelemetry();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowClient");
        app.UseConfiguredHangfire();
        app.UseHealthCheckConfiguration();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}