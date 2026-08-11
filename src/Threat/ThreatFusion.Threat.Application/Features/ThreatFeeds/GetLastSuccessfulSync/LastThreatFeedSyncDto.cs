namespace ThreatFusion.Threat.Application
    .Features.ThreatFeeds.GetLastSuccessfulSync;

public sealed record LastThreatFeedSyncDto(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc);