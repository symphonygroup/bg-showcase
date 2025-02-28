using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(config => 
    config.AddMyTransport(builder.Configuration));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMyOpenTelemetry("api", builder.Configuration);

var app = builder.Build();
// Run migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok());

app.MapGet("/customers/{id}", 
    async ([FromRoute] int id, [FromServices]AppDbContext db) => 
    await db.Customers.FindAsync(id));

app.MapPost("/customers",
    async ([FromBody] Customer customer, 
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] AppDbContext db) =>
    {
        // Save to DB
        await db.Customers.AddAsync(customer);
        await db.SaveChangesAsync();
        // Send CustomerCreated event
        await publishEndpoint.Publish<CustomerCreated>(customer);
    });

app.Run();