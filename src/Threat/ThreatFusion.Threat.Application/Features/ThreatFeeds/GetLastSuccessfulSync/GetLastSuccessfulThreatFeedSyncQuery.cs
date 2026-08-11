using MediatR;

namespace ThreatFusion.Threat.Application
    .Features.ThreatFeeds.GetLastSuccessfulSync;

public sealed record GetLastSuccessfulThreatFeedSyncQuery(
    string FeedName)
    : IRequest<LastThreatFeedSyncDto?>;