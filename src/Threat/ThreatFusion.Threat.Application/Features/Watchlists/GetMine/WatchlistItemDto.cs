namespace ThreatFusion.Threat.Application.Features.Watchlists.GetMine;

public sealed record WatchlistItemDto(
    long WatchlistId,
    long IndicatorId,
    string Type,
    string Value,
    string Severity,
    double RiskScore,
    string RiskLevel,
    string SourceName,
    string? Note,
    DateTime AddedAtUtc);