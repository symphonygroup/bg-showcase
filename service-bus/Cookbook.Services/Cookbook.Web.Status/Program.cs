using Cookbook.Web.Status;
using Cookbook.Web.Status.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7167/") });
builder.Services.AddTransient<IHealthCheckService, HealthCheckService>();

await builder.Build().RunAsync();
