namespace ThreatFusion.FeedCollectorService.Models;

public sealed record ThreatIndicatorRequest(
    int Type,
    string Value,
    int Severity,
    int Confidence,
    string SourceName,
    string? Description,
    DateTime? FirstSeenUtc,
    DateTime? LastSeenUtc);