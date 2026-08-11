namespace ThreatFusion.FeedCollectorService.Models;

public sealed record ThreatFeedSyncRequest(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime CompletedAtUtc,
    int TotalFetched,
    int CreatedCount,
    int UpdatedCount,
    int UnchangedCount,
    int FailedCount,
    bool IsSuccessful,
    string? ErrorMessage);