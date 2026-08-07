using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Domain.Entities;

namespace ThreatFusion.Threat.Application.Abstractions;

public interface IThreatDbContext
{
    DbSet<ThreatIndicator> ThreatIndicators { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}