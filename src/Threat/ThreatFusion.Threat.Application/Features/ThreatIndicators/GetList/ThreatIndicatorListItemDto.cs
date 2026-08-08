namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.GetList;

public sealed record ThreatIndicatorListItemDto(
    long Id,
    string Type,
    string Value,
    string Severity,
    int Confidence,
    string SourceName,
    DateTime? FirstSeenUtc,
    DateTime? LastSeenUtc,
    bool IsActive);