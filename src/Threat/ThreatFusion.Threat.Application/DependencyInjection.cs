using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ThreatFusion.Threat.Application.Services;

namespace ThreatFusion.Threat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly =
            typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                assembly);
        });

        services.AddValidatorsFromAssembly(
            assembly);

        services.AddScoped<DnsEnrichmentService>();

        return services;
    }
}