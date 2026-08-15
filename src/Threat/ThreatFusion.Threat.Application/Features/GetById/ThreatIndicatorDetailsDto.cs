namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.GetById;

public sealed record ThreatIndicatorDetailsDto(
    long Id,
    string Type,
    string Value,
    string Severity,
    int Confidence,
    string SourceName,
    string? Description,
    DateTime? FirstSeenUtc,
    DateTime? LastSeenUtc,
    double? CvssScore,
    string? CvssVersion,
    string? CvssVector,
    string? CweId,
    string? ReferenceUrl,
    bool IsActive
);