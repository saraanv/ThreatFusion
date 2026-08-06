namespace ThreatFusion.Identity.Domain.Common;

/// <summary>
/// رابط حذف نرم.
/// این رابط شامل ویژگی‌هایی برای مدیریت وضعیت حذف نرم موجودیت‌ها است.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// وضعیت حذف موجودیت.
    ///مقدار True نشان‌دهنده حذف‌شدگی و False نشان‌دهنده فعال بودن است.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// تاریخ و زمان حذف موجودیت (در صورت وجود).
    /// </summary>

    DateTime? DeletedAtUtc { get; set; }
}