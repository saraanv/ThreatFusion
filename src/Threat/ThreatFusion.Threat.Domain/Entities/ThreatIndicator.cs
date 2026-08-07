using ThreatFusion.Threat.Domain.Common;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Domain.Entities;

public sealed class ThreatIndicator : BaseEntity
{
    public IndicatorType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public ThreatSeverity Severity { get; set; }

    public int Confidence { get; set; }

    public string? Description { get; set; }

    public string SourceName { get; set; } = string.Empty;

    public DateTime? FirstSeenUtc { get; set; }

    public DateTime? LastSeenUtc { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public bool IsActive { get; set; } = true;
}