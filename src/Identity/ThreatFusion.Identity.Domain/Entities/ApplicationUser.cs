using Microsoft.AspNetCore.Identity;
using ThreatFusion.Identity.Domain.Common;

namespace ThreatFusion.Identity.Domain.Entities;

public sealed class ApplicationUser : IdentityUser<long>, IAuditableEntity, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public int RegDate { get; set; }

    public string RegTime { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAtUtc { get; set; }
}