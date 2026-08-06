using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThreatFusion.Identity.Domain.Entities;
using ThreatFusion.Identity.Infrastructure.Persistence;

namespace ThreatFusion.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'IdentityDatabase' was not found.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<long>>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
}