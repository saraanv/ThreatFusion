using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Identity.Domain.Entities;

namespace ThreatFusion.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(user => user.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.RegTime)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(user => user.CreatedAtUtc)
                .IsRequired();

            entity.Property(user => user.IsActive)
                .IsRequired();

            entity.Property(user => user.IsDeleted)
                .IsRequired();

            entity.HasQueryFilter(user => !user.IsDeleted);
        });

        builder.Entity<IdentityRole<long>>()
            .ToTable("Roles");

        builder.Entity<IdentityUserRole<long>>()
            .ToTable("UserRoles");

        builder.Entity<IdentityUserClaim<long>>()
            .ToTable("UserClaims");

        builder.Entity<IdentityUserLogin<long>>()
            .ToTable("UserLogins");

        builder.Entity<IdentityRoleClaim<long>>()
            .ToTable("RoleClaims");

        builder.Entity<IdentityUserToken<long>>()
            .ToTable("UserTokens");
    }
}