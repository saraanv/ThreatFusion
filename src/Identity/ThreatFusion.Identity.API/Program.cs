using ThreatFusion.Identity.Application;
using ThreatFusion.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDataProtection();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Service = "ThreatFusion.Identity.API",
    TimestampUtc = DateTime.UtcNow
}));

app.Run();