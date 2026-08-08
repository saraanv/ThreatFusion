namespace ThreatFusion.FeedCollectorService.Models;

public sealed record ThreatFeedSyncRequest(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime CompletedAtUtc,
    int TotalFetched,
    int ImportedCount,
    int SkippedCount,
    int FailedCount,
    bool IsSuccessful,
    string? ErrorMessage);