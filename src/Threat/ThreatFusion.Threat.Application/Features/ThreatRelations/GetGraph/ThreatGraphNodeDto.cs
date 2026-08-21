namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed record ThreatGraphNodeDto(
    long Id,
    string Type,
    string Value,
    string Severity,
    double RiskScore,
    string RiskLevel,
    string SourceName);