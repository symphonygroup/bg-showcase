using Cookbook.Web.Blazor;
using Cookbook.Web.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7167/") });
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IRecipeService, RecipeService>();
builder.Services.AddTransient<ICookingService, CookingService>();


await builder.Build().RunAsync();
