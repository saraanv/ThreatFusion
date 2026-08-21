namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetByIndicator;

public sealed record ThreatRelationDto(
    long RelationId,
    long SourceIndicatorId,
    string SourceValue,
    long TargetIndicatorId,
    string TargetValue,
    string RelationType,
    string? Description,
    double Confidence,
    bool IsActive);