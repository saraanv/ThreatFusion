namespace ThreatFusion.Threat.Application.Features.Dashboard.GetOverview;

public sealed record DashboardRecentAlertDto(
    long Id,
    long ThreatIndicatorId,
    string IndicatorValue,
    string AlertType,
    string Title,
    string Severity,
    bool IsRead,
    DateTime CreatedAtUtc);