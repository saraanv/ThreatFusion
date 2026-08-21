using ThreatFusion.Threat.Domain.Common;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Domain.Entities;

public sealed class ThreatIndicatorRelation : BaseEntity
{
    public long SourceIndicatorId { get; set; }

    public long TargetIndicatorId { get; set; }

    public ThreatRelationType RelationType { get; set; }

    public string? Description { get; set; }

    public double Confidence { get; set; }

    public bool IsActive { get; set; } = true;

    public string SourceName { get; set; } = string.Empty;

    public bool IsAutomatic { get; set; }

    public DateTime DiscoveredAtUtc { get; set; }
}