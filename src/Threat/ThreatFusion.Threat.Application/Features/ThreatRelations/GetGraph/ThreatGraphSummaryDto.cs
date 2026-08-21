namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed record ThreatGraphSummaryDto(
    int NodeCount,
    int EdgeCount,
    int CriticalNodeCount,
    int HighRiskNodeCount,
    int AutomaticRelationCount,
    int ManualRelationCount,
    double AverageRiskScore,
    long? HighestRiskIndicatorId,
    string? HighestRiskIndicatorValue,
    double? HighestRiskScore);