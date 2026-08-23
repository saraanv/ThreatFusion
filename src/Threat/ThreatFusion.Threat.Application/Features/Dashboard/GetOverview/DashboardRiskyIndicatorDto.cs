namespace ThreatFusion.Threat.Application.Features.Dashboard.GetOverview;

public sealed record DashboardRiskyIndicatorDto(
    long Id,
    string Type,
    string Value,
    string Severity,
    double RiskScore,
    string RiskLevel,
    string SourceName);