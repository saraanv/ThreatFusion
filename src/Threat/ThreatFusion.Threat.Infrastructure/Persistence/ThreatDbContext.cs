using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Infrastructure.Persistence;

public sealed class ThreatDbContext
    : DbContext, IThreatDbContext
{
    public ThreatDbContext(
        DbContextOptions<ThreatDbContext> options)
        : base(options)
    {
    }

    public DbSet<ThreatIndicator> ThreatIndicators =>
        Set<ThreatIndicator>();

    public DbSet<ThreatFeedSync> ThreatFeedSyncs =>
        Set<ThreatFeedSync>();

    public DbSet<ThreatIndicatorRelation>
        ThreatIndicatorRelations =>
        Set<ThreatIndicatorRelation>();

    public DbSet<ThreatWatchlist> ThreatWatchlists =>
        Set<ThreatWatchlist>();

    public DbSet<ThreatAlert> ThreatAlerts =>
        Set<ThreatAlert>();

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /*
         * ==========================================
         * THREAT INDICATORS
         * ==========================================
         */
        builder.Entity<ThreatIndicator>(entity =>
        {
            entity.ToTable("ThreatIndicators");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Value)
                .HasMaxLength(2048)
                .IsRequired();

            entity.Property(x => x.SourceName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            entity.Property(x => x.Type)
                .IsRequired();

            entity.Property(x => x.Severity)
                .IsRequired();

            entity.Property(x => x.Confidence)
                .IsRequired();

            entity.Property(x => x.CvssVersion)
                .HasMaxLength(20);

            entity.Property(x => x.CvssVector)
                .HasMaxLength(500);

            entity.Property(x => x.CweId)
                .HasMaxLength(50);

            entity.Property(x => x.ReferenceUrl)
                .HasMaxLength(2048);

            entity.HasIndex(x => new
            {
                x.Type,
                x.Value
            })
            .IsUnique();

            /*
             * Dashboard performance
             */
            entity.HasIndex(x => new
            {
                x.IsDeleted,
                x.IsActive,
                x.RiskScore
            });

            entity.HasIndex(x => new
            {
                x.IsDeleted,
                x.IsActive,
                x.RiskLevel
            });

            entity.HasQueryFilter(x =>
                !x.IsDeleted);
        });

        /*
         * ==========================================
         * FEED SYNCS
         * ==========================================
         */
        builder.Entity<ThreatFeedSync>(entity =>
        {
            entity.ToTable("ThreatFeedSyncs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FeedName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);
        });

        /*
         * ==========================================
         * THREAT RELATIONS
         * ==========================================
         */
        builder.Entity<ThreatIndicatorRelation>(
            entity =>
            {
                entity.ToTable(
                    "ThreatIndicatorRelations");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Confidence)
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .IsRequired();

                entity.Property(x => x.SourceName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.IsAutomatic)
                    .IsRequired();

                entity.Property(x =>
                        x.DiscoveredAtUtc)
                    .IsRequired();

                entity.HasIndex(x => new
                {
                    x.SourceIndicatorId,
                    x.TargetIndicatorId,
                    x.RelationType
                })
                .IsUnique();

                entity.HasIndex(x => new
                {
                    x.IsDeleted,
                    x.IsActive,
                    x.IsAutomatic
                });
            });

        /*
         * ==========================================
         * WATCHLIST
         * ==========================================
         */
        builder.Entity<ThreatWatchlist>(entity =>
        {
            entity.ToTable("ThreatWatchlists");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Note)
                .HasMaxLength(1000);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.UserId,
                x.ThreatIndicatorId
            })
            .IsUnique();

            entity.HasIndex(x => new
            {
                x.UserId,
                x.IsActive,
                x.IsDeleted
            });
        });

        /*
         * ==========================================
         * ALERTS
         * ==========================================
         */
        builder.Entity<ThreatAlert>(entity =>
        {
            entity.ToTable("ThreatAlerts");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .HasMaxLength(300)
                .IsRequired();

            entity.Property(x => x.Message)
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(x => x.AlertType)
                .IsRequired();

            entity.Property(x => x.Severity)
                .IsRequired();

            entity.Property(x => x.IsRead)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.UserId,
                x.IsRead,
                x.IsDeleted
            });

            entity.HasIndex(x => new
            {
                x.UserId,
                x.CreatedAtUtc
            });

            entity.HasIndex(x =>
                x.ThreatIndicatorId);
        });
    }
}