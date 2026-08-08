namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed record FeedSyncSummaryDto(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    int TotalFetched,
    int ImportedCount,
    int SkippedCount,
    int FailedCount,
    bool IsSuccessful);