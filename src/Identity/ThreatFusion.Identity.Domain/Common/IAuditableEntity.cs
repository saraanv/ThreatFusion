namespace ThreatFusion.Identity.Domain.Common;

/// <summary>
/// رابط ویژگی های مانند تاریخ ایجاد
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// تاریخ ثبت
    /// </summary>
    int RegDate { get; set; }

    /// <summary>
    /// زمان ثبت
    /// </summary>
    string RegTime { get; set; }

    /// <summary>
    /// تاریخ ایجاد
    /// </summary>
    DateTime CreatedAtUtc { get; set; }
}