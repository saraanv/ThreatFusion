namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.GetList;

public sealed record ThreatIndicatorListItemDto(
    long Id,
    string Type,
    string Value,
    string Severity,
    int Confidence,
    double RiskScore,
    string RiskLevel,
    string SourceName,
    string? Description,
    DateTime? FirstSeenUtc,
    DateTime? LastSeenUtc,
    double? CvssScore,
    string? CvssVersion,
    string? CvssVector,
    string? CweId,
    string? ReferenceUrl,
    bool IsActive);