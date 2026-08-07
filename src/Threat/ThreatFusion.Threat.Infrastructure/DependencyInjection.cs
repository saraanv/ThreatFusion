using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Infrastructure.Persistence;

namespace ThreatFusion.Threat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("ThreatDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'ThreatDatabase' was not found.");

        services.AddDbContext<ThreatDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IThreatDbContext>(
            provider => provider.GetRequiredService<ThreatDbContext>());
        
        return services;
    }
}