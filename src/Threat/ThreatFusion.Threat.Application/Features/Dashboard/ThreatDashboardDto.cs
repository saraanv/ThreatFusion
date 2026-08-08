namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed record ThreatDashboardDto(
    int TotalIndicators,
    int ActiveIndicators,
    int CriticalIndicators,
    IReadOnlyCollection<IndicatorTypeCountDto> IndicatorsByType,
    IReadOnlyCollection<SourceCountDto> IndicatorsBySource,
    IReadOnlyCollection<LatestThreatDto> LatestThreats,
    FeedSyncSummaryDto? LastFeedSync);