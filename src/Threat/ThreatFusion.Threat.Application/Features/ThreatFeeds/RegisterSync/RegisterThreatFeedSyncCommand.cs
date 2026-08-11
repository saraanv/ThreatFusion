using MediatR;

namespace ThreatFusion.Threat.Application
    .Features.ThreatFeeds.RegisterSync;

public sealed record RegisterThreatFeedSyncCommand(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime CompletedAtUtc,
    int TotalFetched,
    int CreatedCount,
    int UpdatedCount,
    int UnchangedCount,
    int FailedCount,
    bool IsSuccessful,
    string? ErrorMessage)
    : IRequest<RegisterThreatFeedSyncResult>;