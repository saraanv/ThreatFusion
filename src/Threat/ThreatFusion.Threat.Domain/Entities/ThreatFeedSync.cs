using ThreatFusion.Threat.Domain.Common;

namespace ThreatFusion.Threat.Domain.Entities;

public sealed class ThreatFeedSync : BaseEntity
{
    public string FeedName { get; set; } = string.Empty;

    public DateTime StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public int TotalFetched { get; set; }

    public int CreatedCount { get; set; }

    public int UpdatedCount { get; set; }

    public int UnchangedCount { get; set; }

    public int FailedCount { get; set; }

    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }
}