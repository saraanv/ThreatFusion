using ThreatFusion.Threat.Domain.Enums;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.Search;

public sealed record ThreatIndicatorDto(
    long Id,
    IndicatorType Type,
    string Value,
    ThreatSeverity Severity,
    int Confidence,
    string SourceName,
    string? Description,
    DateTime? FirstSeenUtc,
    DateTime? LastSeenUtc,
    bool IsActive);