using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application.Abstractions;
using ThreatFusion.Threat.Domain.Entities;

public interface IThreatDbContext
{
    DbSet<ThreatIndicator> ThreatIndicators { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
    DbSet<ThreatFeedSync> ThreatFeedSyncs { get; }
    DbSet<ThreatIndicatorRelation> ThreatIndicatorRelations { get; }
    DbSet<ThreatWatchlist> ThreatWatchlists { get; }
}