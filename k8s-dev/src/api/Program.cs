var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
  options.AddDefaultPolicy(policy => {
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
  });
});
var app = builder.Build();
app.UseCors();

app.MapGet("/", () => Results.Ok(new { message = "Hello World 2.59!"}));

app.Run();
