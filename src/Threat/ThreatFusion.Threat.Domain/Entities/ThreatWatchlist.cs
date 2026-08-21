using ThreatFusion.Threat.Domain.Common;

namespace ThreatFusion.Threat.Domain.Entities;

public sealed class ThreatWatchlist : BaseEntity
{
    public long UserId { get; set; }

    public long ThreatIndicatorId { get; set; }

    public string? Note { get; set; }

    public bool IsActive { get; set; } = true;
}