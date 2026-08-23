namespace ThreatFusion.Threat.Application.Features.Dashboard.GetOverview;

public sealed record DashboardOverviewDto(
    int TotalIndicators,
    int CriticalIndicators,
    int HighRiskIndicators,
    int WatchedIndicators,
    int UnreadAlerts,
    int AutomaticRelations,
    int ManualRelations,
    IReadOnlyCollection<DashboardRiskyIndicatorDto> TopRiskyIndicators,
    IReadOnlyCollection<DashboardRecentAlertDto> RecentAlerts);