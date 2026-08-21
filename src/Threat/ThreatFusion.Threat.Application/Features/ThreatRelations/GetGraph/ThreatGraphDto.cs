namespace ThreatFusion.Threat.Application.Features.ThreatRelations.GetGraph;

public sealed record ThreatGraphDto(
    IReadOnlyCollection<ThreatGraphNodeDto> Nodes,
    IReadOnlyCollection<ThreatGraphEdgeDto> Edges);