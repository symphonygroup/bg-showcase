using System.Net.Http.Json;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<WebApiClient>(client => 
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("WebApi:Url")
    ?? throw new InvalidOperationException("WebApi:Url is missing")));

builder.Services.AddMassTransit(config =>
    {
        config.AddMyTransport(builder.Configuration);
        config.AddConsumer<CustomerCreatedConsumer>();
    });

builder.Services.AddMyOpenTelemetry("worker", builder.Configuration);

var app = builder.Build();

await app.RunAsync();

public class CustomerCreatedConsumer(WebApiClient webApiClient) : IConsumer<CustomerCreated>
{
    public async Task Consume(ConsumeContext<CustomerCreated> context)
    {
        // call WebApi back to retrieve data
        var result = await webApiClient.GetCustomerAsync(context.Message.Id);
        if (result != null)
        {
            Console.WriteLine($"Customer Retrieved: {result.Id}, {result.Name}");
        }
        else
        {
            throw new Exception($"Customer not found: {context.Message.Id}");
        }
    }
}

public class WebApiClient(HttpClient httpClient)
{
    public async Task<Customer?> GetCustomerAsync(int id)
    {
        var response = await httpClient.GetAsync($"/customers/{id}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<Customer>();
    }
}