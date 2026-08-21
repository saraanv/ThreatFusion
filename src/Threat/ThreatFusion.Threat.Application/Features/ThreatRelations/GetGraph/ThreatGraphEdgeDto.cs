namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed record ThreatGraphEdgeDto(
    long RelationId,
    long SourceId,
    long TargetId,
    string RelationType,
    double Confidence,
    string? Description,
    string SourceName,
    bool IsAutomatic,
    DateTime DiscoveredAtUtc);