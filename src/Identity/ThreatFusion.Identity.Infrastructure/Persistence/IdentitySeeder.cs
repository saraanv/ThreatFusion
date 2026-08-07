using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThreatFusion.Identity.Domain.Constants;
using ThreatFusion.Identity.Domain.Entities;

namespace ThreatFusion.Identity.Infrastructure.Persistence;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<long>>>();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRolesAsync(roleManager);

        await SeedAdminAsync(
            userManager,
            configuration);

        await SeedFeedCollectorAccountAsync(
            userManager,
            configuration);
    }

    private static async Task SeedRolesAsync(
        RoleManager<IdentityRole<long>> roleManager)
    {
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

    private static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email =
            configuration["InitialAdmin:Email"];

        var password =
            configuration["InitialAdmin:Password"];

        var firstName =
            configuration["InitialAdmin:FirstName"];

        var lastName =
            configuration["InitialAdmin:LastName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Initial admin configuration is missing.");
        }

        var existingUser =
            await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            if (!await userManager.IsInRoleAsync(
                    existingUser,
                    Roles.Admin))
            {
                await userManager.AddToRoleAsync(
                    existingUser,
                    Roles.Admin);
            }

            return;
        }

        var now = DateTime.UtcNow;

        var admin = new ApplicationUser
        {
            FirstName =
                firstName ?? "ThreatFusion",

            LastName =
                lastName ?? "Administrator",

            Email = email,

            UserName = email,

            CreatedAtUtc = now,

            RegDate =
                int.Parse(now.ToString("yyyyMMdd")),

            RegTime =
                now.ToString("HH:mm"),

            IsActive = true,

            IsDeleted = false
        };

        var createResult =
            await userManager.CreateAsync(
                admin,
                password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createResult.Errors.Select(
                    x => x.Description));

            throw new InvalidOperationException(
                $"Could not create initial admin: {errors}");
        }

        await userManager.AddToRoleAsync(
            admin,
            Roles.Admin);
    }
    
    private static async Task SeedFeedCollectorAccountAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email =
            configuration["FeedCollectorAccount:Email"];

        var password =
            configuration["FeedCollectorAccount:Password"];

        var firstName =
            configuration["FeedCollectorAccount:FirstName"];

        var lastName =
            configuration["FeedCollectorAccount:LastName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "FeedCollector account configuration is missing.");
        }

        var existingUser =
            await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            if (!await userManager.IsInRoleAsync(
                    existingUser,
                    Roles.Analyst))
            {
                await userManager.AddToRoleAsync(
                    existingUser,
                    Roles.Analyst);
            }

            return;
        }

        var now = DateTime.UtcNow;

        var user = new ApplicationUser
        {
            FirstName = firstName ?? "ThreatFusion",
            LastName = lastName ?? "FeedCollector",

            Email = email,
            UserName = email,

            CreatedAtUtc = now,

            RegDate =
                int.Parse(now.ToString("yyyyMMdd")),

            RegTime =
                now.ToString("HH:mm"),

            IsActive = true,
            IsDeleted = false
        };

        var createResult =
            await userManager.CreateAsync(
                user,
                password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createResult.Errors.Select(
                    x => x.Description));

            throw new InvalidOperationException(
                $"Could not create FeedCollector account: {errors}");
        }

        await userManager.AddToRoleAsync(
            user,
            Roles.Analyst);
    }
}