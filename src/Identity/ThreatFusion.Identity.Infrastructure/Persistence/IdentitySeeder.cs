using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ThreatFusion.Identity.Domain.Constants;

namespace ThreatFusion.Identity.Infrastructure.Persistence;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<long>>>();

        string[] roles =
        [
            Roles.Admin,
            Roles.Analyst,
            Roles.Viewer
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<long>(role));
            }
        }
    }
}