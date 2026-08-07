namespace ThreatFusion.Threat.Domain.Common;

public abstract class BaseEntity : IAuditableEntity, ISoftDelete
{
    public long Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAtUtc { get; set; }
}