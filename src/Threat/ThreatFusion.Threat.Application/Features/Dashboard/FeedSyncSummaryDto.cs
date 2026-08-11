namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed record FeedSyncSummaryDto(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    int TotalFetched,
    int CreatedCount,
    int UpdatedCount,
    int UnchangedCount,
    int FailedCount,
    bool IsSuccessful);