namespace ThreatFusion.Threat.Domain.Common;

public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }
}