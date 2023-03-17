namespace Cookbook.Api;

using Application.Extensions;
using Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServices(Configuration);
        services.AddOpenTelemetry();
        services.ConfigureHangfire(Configuration);
        services.ConfigureMassTransitWithRabbit(Configuration);
        services.AddSwaggerConfiguration();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerConfiguration();
        }

        app.UseCors("AllowClient");
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseConfiguredHangfire();
        app.UseHealthCheckConfiguration();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}