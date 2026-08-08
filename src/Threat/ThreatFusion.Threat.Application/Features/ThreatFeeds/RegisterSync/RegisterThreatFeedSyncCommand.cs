using MediatR;

namespace ThreatFusion.Threat.Application
    .Features.ThreatFeeds.RegisterSync;

public sealed record RegisterThreatFeedSyncCommand(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime CompletedAtUtc,
    int TotalFetched,
    int ImportedCount,
    int SkippedCount,
    int FailedCount,
    bool IsSuccessful,
    string? ErrorMessage)
    : IRequest<RegisterThreatFeedSyncResult>;