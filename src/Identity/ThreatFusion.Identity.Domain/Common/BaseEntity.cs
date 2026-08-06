using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreatFusion.Identity.Domain.Common;

/// <summary>
/// کلاس پایه برای موجودیت های دامنه رایج
/// </summary>
public abstract class BaseEntity : IAuditableEntity, ISoftDelete
{
    /// <summary>
    /// شناسه یکتا
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// تاریخ ثبت
    /// </summary>
    public int RegDate { get; set; }

    /// <summary>
    /// زمان ثبت
    /// </summary>
    public string RegTime { get; set; } = string.Empty;

    /// <summary>
    /// تاریخ ایجاد
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// بررسی حذف شده بودن؟
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// تازیخ و زمان ثبت
    /// </summary>
    public DateTime? DeletedAtUtc { get; set; }
}