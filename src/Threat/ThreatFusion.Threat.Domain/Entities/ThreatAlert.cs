using ThreatFusion.Threat.Domain.Common;
using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Domain.Entities;

public sealed class ThreatAlert : BaseEntity
{
    public long UserId { get; set; }

    public long ThreatIndicatorId { get; set; }

    public ThreatAlertType AlertType { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public ThreatSeverity Severity { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAtUtc { get; set; }
}