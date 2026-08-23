namespace ThreatFusion.Threat.Application.Features.Dashboard.GetOverview;

public sealed record DashboardFeedSyncDto(
    string FeedName,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    int TotalFetched,
    int FailedCount,
    bool IsSuccessful,
    string? ErrorMessage);