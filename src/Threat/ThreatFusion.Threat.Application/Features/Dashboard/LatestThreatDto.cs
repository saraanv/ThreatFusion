namespace ThreatFusion.Threat.Application.Features.Dashboard;

public sealed record LatestThreatDto(
    long Id,
    string Type,
    string Value,
    string Severity,
    int Confidence,
    string SourceName,
    DateTime? FirstSeenUtc);