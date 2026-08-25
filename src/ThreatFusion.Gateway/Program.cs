var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Frontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("Frontend");

app.MapReverseProxy();

app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Service = "ThreatFusion.Gateway",
    TimestampUtc = DateTime.UtcNow
}));

app.Run();